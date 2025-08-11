using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace PetWeb.Services
{
    public class LocalImageStorageService : IImageStorageService
    {
        private readonly string _root;
        private readonly IWebHostEnvironment _env;

        public LocalImageStorageService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            var configuredRoot = configuration["Storage:Local:Root"] ?? "wwwroot/pet-images";
            _root = Path.IsPathRooted(configuredRoot) ? configuredRoot : Path.Combine(env.ContentRootPath, configuredRoot);
            Directory.CreateDirectory(_root);
        }

        public async Task<string?> UploadAsync(IFormFile file, string fileNameHint)
        {
            if (file == null || file.Length == 0) return null;
            var extension = Path.GetExtension(file.FileName);
            var safeName = string.Join("-", fileNameHint.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant();
            var name = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{safeName}{extension}";
            var savePath = Path.Combine(_root, name);
            using var stream = new FileStream(savePath, FileMode.Create);
            await file.CopyToAsync(stream);
            var relativePath = $"/pet-images/{name}";
            return relativePath;
        }

        public Task DeleteAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl)) return Task.CompletedTask;
                if (!imageUrl.StartsWith("/pet-images/", StringComparison.OrdinalIgnoreCase)) return Task.CompletedTask;
                var name = imageUrl.Substring("/pet-images/".Length);
                var path = Path.Combine(_root, name);
                if (File.Exists(path)) File.Delete(path);
            }
            catch
            {
                // ignore on delete
            }
            return Task.CompletedTask;
        }
    }
}
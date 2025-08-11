using System;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace PetWeb.Services
{
    public class AzureBlobImageStorageService : IImageStorageService
    {
        private readonly BlobContainerClient _container;
        private readonly string? _publicBaseUrl;

        public AzureBlobImageStorageService(IConfiguration configuration)
        {
            var conn = configuration["Storage:Azure:ConnectionString"];
            var serviceUrl = configuration["Storage:Azure:BlobServiceUrl"]; // https://account.blob.core.windows.net
            var containerName = configuration["Storage:Azure:ContainerName"] ?? "pet-images";

            if (!string.IsNullOrWhiteSpace(conn))
            {
                var service = new BlobServiceClient(conn);
                _container = service.GetBlobContainerClient(containerName);
            }
            else if (!string.IsNullOrWhiteSpace(serviceUrl))
            {
                var service = new BlobServiceClient(new Uri(serviceUrl), new DefaultAzureCredential());
                _container = service.GetBlobContainerClient(containerName);
            }
            else
            {
                throw new InvalidOperationException("Azure Blob configuration missing. Set Storage:Azure:ConnectionString or Storage:Azure:BlobServiceUrl");
            }

            _container.CreateIfNotExists(PublicAccessType.Blob);
            _publicBaseUrl = _container.Uri.ToString().TrimEnd('/');
        }

        public async Task<string?> UploadAsync(IFormFile file, string fileNameHint)
        {
            if (file == null || file.Length == 0) return null;
            var extension = Path.GetExtension(file.FileName);
            var safeName = string.Join("-", fileNameHint.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant();
            var blobName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{safeName}{extension}";

            var blob = _container.GetBlobClient(blobName);
            await blob.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

            return _publicBaseUrl + "/" + Uri.EscapeDataString(blobName);
        }

        public async Task DeleteAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl)) return;
                // imageUrl likely like https://.../container/blobName
                var blobName = new Uri(imageUrl).Segments[^1];
                var blob = _container.GetBlobClient(Uri.UnescapeDataString(blobName));
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }
}
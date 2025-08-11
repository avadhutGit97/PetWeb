using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PetWeb.Services
{
    public interface IImageStorageService
    {
        Task<string?> UploadAsync(IFormFile file, string fileNameHint);
        Task DeleteAsync(string imageUrl);
    }
}
using Microsoft.AspNetCore.Http;

namespace Services.Services.Inerfaces
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string path = "");
    }
}

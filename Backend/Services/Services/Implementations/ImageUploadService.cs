using Microsoft.AspNetCore.Http;
using Repository.ImageUploadService.Exceptions;
using Services.Services.Inerfaces;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Services.Services.Implementations
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly string[] SupportedTypes = { ".png", ".jpg", ".jpeg", ".webp", ".bmp" };

        public async Task<string> UploadImageAsync(IFormFile file, string path = "")
        {
            string extension = Path.GetExtension(file.FileName);

            if (!SupportedTypes.Contains(extension))
                throw new UnsupportedFileExtensionException($"{extension} is unsupported file extension, upload {string.Join(", ", SupportedTypes)}");

            string filename = Guid.NewGuid().ToString();
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot//images//avatars//", filename);

            var image = await CropImageAsync(file);

            using (FileStream fileStream = File.Create(uploadPath + extension))
            {
                image.Save(fileStream, new JpegEncoder());
                fileStream.Flush();
            }
            image.Dispose();
            return filename + extension;
        }

        private async Task<Image> CropImageAsync(IFormFile file)
        {
            var image = await Image.LoadAsync(file.OpenReadStream());
            var size = Math.Min(image.Width, image.Height);
            var cropRectangle = new Rectangle((image.Width - size) / 2, (image.Height - size) / 2, size, size);
            image.Mutate(x => x.Crop(cropRectangle));
            return image;
        }
    }
}

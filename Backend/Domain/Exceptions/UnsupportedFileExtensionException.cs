
namespace Repository.ImageUploadService.Exceptions
{
    public class UnsupportedFileExtensionException : Exception
    {
        public UnsupportedFileExtensionException(string message) : base(message) { }
    }
}

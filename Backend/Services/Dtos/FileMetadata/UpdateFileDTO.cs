
using Domain.Enums;
using Services.Dtos.User;

namespace Services.Dtos.FileMetadata
{
    public class UpdateFileDTO
    {
        public string? Name { get; set; }
        public FileAccessabilities? Accesability { get; set; }
        public IEnumerable<int>? PermittedUsers { get; set; }
    }
}

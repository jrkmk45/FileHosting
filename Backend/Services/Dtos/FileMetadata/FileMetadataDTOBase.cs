using Domain.Enums;
using Services.Dtos.User;

namespace Services.Dtos.FileMetadata
{
    public class FileMetadataDTOBase
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public FileAccessabilities Accessability { get; set; }
        public UserDTO Owner { get; set; }

        public DateTime CreatedDate { get; set; }
        
    }
}

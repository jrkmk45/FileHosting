using Services.Dtos.User;

namespace Services.Dtos.FileMetadata
{
    public class FileFullDTO : FileMetadataDTOBase
    {
        public IEnumerable<UserDTO> PermittedUsers { get; set; }
    }
}

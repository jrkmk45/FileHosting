using Services.Dtos.User;

namespace Services.Dtos.FileMetadata
{
    public class SingleFileMetadataDTO : FileMetadataDTOBase
    {
        public UserDTO User { get; set; }
    }
}

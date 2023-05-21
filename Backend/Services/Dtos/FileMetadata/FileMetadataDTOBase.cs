
namespace Services.Dtos.FileMetadata
{
    public class FileMetadataDTOBase
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public bool IsPrivate { get; set; }

        public DateTime CreatedDate { get; set; }
        
    }
}

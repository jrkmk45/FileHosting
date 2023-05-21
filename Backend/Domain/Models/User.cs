using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class User : IdentityUser<int>
    {
        public IEnumerable<FileMetadata> Files { get; set; }
        public List<FileMetadata> AccessedFiles { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? ProfilePicture { get; set; }

    }
}

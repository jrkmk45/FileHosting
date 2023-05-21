using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class FileMetadata
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        public string StorageName { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public bool isPrivate { get; set; }
        public List<User> PermittedUsers { get; set; }
    }
}

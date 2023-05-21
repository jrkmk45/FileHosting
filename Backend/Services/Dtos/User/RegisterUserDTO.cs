using System.ComponentModel.DataAnnotations;

namespace Services.Dtos.User
{
    public class RegisterUserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

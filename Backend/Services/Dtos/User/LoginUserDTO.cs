using System.ComponentModel.DataAnnotations;

namespace Services.Dtos.User
{
    public class LoginUserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}

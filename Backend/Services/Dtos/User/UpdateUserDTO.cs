
using Microsoft.AspNetCore.Http;

namespace Services.Dtos.User
{
    public class UpdateUserDTO
    {
        public IFormFile? ProfilePicture { get; set; }
    }
}

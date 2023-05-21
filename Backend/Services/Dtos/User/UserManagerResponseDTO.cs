
namespace Services.Dtos.User
{
    public class UserManagerResponseDTO
    {
        public string Message { get; set; }
        public string? Token { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public DateTime? TokenExpirationDate { get; set; }
    }
}

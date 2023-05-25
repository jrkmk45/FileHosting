using Services.Dtos.User;

namespace Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserManagerResponseDTO> AddUserAsync(RegisterUserDTO model);
        Task<UserManagerResponseDTO> LoginUserAsync(LoginUserDTO model);
        Task<UserDTO> GetUserByIdAsync(int id);
        Task UpdateUserAsync(int id, UpdateUserDTO user);
        Task<IEnumerable<UserDTO>> SearchUsersByUserNameAsync(string userName);
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<IEnumerable<UserDTO>> GetNonAccessedUsersByFile(string fileId, string? searchTerm, int userId);
    }
}

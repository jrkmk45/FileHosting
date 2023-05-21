using Services.Dtos.User;

namespace Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserManagerResponseDTO> AddUserAsync(RegisterUserDTO model);
        Task<UserManagerResponseDTO> LoginUserAsync(LoginUserDTO model);
        Task<UserDTO> GetUserByIdAsync(int id);
        Task UpdateUserAsync(int id, UpdateUserDTO user);
    }
}

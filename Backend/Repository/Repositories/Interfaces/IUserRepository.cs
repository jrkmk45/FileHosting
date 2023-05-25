using Domain.Models;

namespace Repository.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User entity);
        Task<User> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
        Task<IEnumerable<User>> SearchUsersByUserNameAsync(string userName);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<IEnumerable<User>> GetNonAccessedUsersByFileAsync(string fileId, int ownerId, string? searchTerm = null);
    }
}

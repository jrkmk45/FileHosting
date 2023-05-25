using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> SearchUsersByUserNameAsync(string userName)
        {
            return await _context.Users.Where(u => u.UserName.ToLower().Contains(userName.ToLower())).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<User>> GetNonAccessedUsersByFileAsync(string fileId, int ownerId, string? searchTerm = null)
        {
            var file = await _context.Files.FindAsync(fileId);
            var users = _context.Users.Where(u => !u.AccessedFiles.Contains(file) && u.Id != ownerId);

            if (searchTerm != null)
                users = users.Where(u => u.UserName.ToLower().Contains(searchTerm.ToLower()));

            return await users.ToListAsync();
        }
    }
}

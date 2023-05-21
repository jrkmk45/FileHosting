using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories.Interfaces;
using System.Linq;

namespace Repository.Repositories.Implementations
{
    public class FileMetadataRepository : IFileMetadataRepository
    {
        private readonly AppDbContext _context;
        public FileMetadataRepository(AppDbContext context) 
        { 
            _context = context;
        }

        public async Task AddFileMetadataAsync(FileMetadata file)
        {
            await _context.AddAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task<FileMetadata> GetFileMetadataAsync(string fileId)
        {
            return await _context.Files
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == fileId);
        }

        public async Task<IEnumerable<FileMetadata>> GetUserFilesMetadataAsync(int userId)
        {
            return await _context.Files
                .Where(f => f.UserId == userId)
                .Include(f => f.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<FileMetadata>> GetUserFilesMetadataAsync(int userId, int ownerId)
        {
            return await _context.Files
                .Where(f => f.UserId == userId)
                .Include(f => f.User)
                .ToListAsync();
        }

        public async Task<FileMetadata> GetFileAsync(string fileId)
        {
            return await _context.Files.FirstOrDefaultAsync(f => f.Id == fileId);
        }

        public async Task<bool> CheckPermission(int userId, string fileId)
        {
            return await _context.Files.AnyAsync(f => f.Id == fileId && f.PermittedUsers.Any(u => u.Id == userId));

            //    .AnyAsync(f => f.PermittedUsers.Contains(user));
            //                .Any(f => f.Id == fileId && f.PermittedUsers.Contains(user));
        }

        public IEnumerable<FileMetadata> SearchFilesByName(string name, int userId)
        {
            var files = _context.Files.Where(f => (f.Name.ToLower() + f.Extension.ToLower())
                .Contains(name.ToLower()));
            /*
            return await _context.Files.Where(f => f.User.Id == userId && f.Name.Contains(name) ||
                f.PermittedUsers.Any(u => u.Id == userId) && f.Name.Contains(name) ||
                f.Name.Contains(name) )*/

            return files.Where(f => !f.isPrivate || f.User.Id == userId || f.PermittedUsers.Any(u => u.Id == userId));
        }

        public async Task<IEnumerable<FileMetadata>> SearchFilesByNameAsync(string name)
        {
            return await _context.Files.Where(f => !f.isPrivate && (f.Name.ToLower()+f.Extension.ToLower())
                .Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task<IEnumerable<FileMetadata>> GetFilesAsync(int? userId)
        {
            if (userId == null)
                return await _context.Files.Where(f => !f.isPrivate).ToListAsync();

            return _context.Files.Where(f => !f.isPrivate || f.User.Id == userId ||
                f.PermittedUsers.Any(u => u.Id == userId));
        }

        public async Task DeleteFileAsync(FileMetadata file)
        {
            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
        }
    }
}

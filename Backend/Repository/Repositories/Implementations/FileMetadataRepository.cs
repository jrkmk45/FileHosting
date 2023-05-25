using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories.Implementations
{
    public class FileMetadataRepository : IFileMetadataRepository
    {
        private readonly AppDbContext _context;
        public FileMetadataRepository(AppDbContext context) 
        { 
            _context = context;
        }

        public async Task AddFile(FileMetadata file)
        {
            await _context.AddAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FileMetadata>> GetUserFilesAsync(int userId)
        {
            return await _context.Files
                .Where(f => f.UserId == userId && f.Accessability == FileAccessabilities.Public)
                .Include(f => f.PermittedUsers)
                .Include(f => f.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<FileMetadata>> GetUserFilesAsync(int userId, int requesterId, 
            FileAccessabilities? accessability = null, string? searchTerm = null)
        {
            var files = _context.Files.Include(f => f.User).Where(f => f.UserId == userId);

            if (searchTerm != null)
                files = files.Where(f => (f.Name + f.Extension).ToLower().Contains(searchTerm.ToLower()));

            if (userId == requesterId)
            {
                if (accessability != null)
                    files = files.Where(f => f.Accessability == accessability);

                return await files.ToListAsync();
            }

            if (accessability == null)
                files = files.Where(f => f.Accessability == FileAccessabilities.Public ||
                f.PermittedUsers.Any(u => u.Id == requesterId));

            if (accessability == FileAccessabilities.ByLink)
                files = files.DefaultIfEmpty();

            if (accessability == FileAccessabilities.Private)
                files = files.Where(f => f.PermittedUsers.Any(u => u.Id == requesterId));

            return await files.ToListAsync();

        }

        public async Task<FileMetadata> GetFileAsync(string fileId)
        {
            return await _context.Files
                .Include(f => f.User)
                .Include(f => f.PermittedUsers)
                .FirstOrDefaultAsync(f => f.Id == fileId);
        }

        public async Task<bool> CheckPermission(int userId, string fileId)
        {
            return await _context.Files.AnyAsync(f => f.Id == fileId && f.PermittedUsers.Any(u => u.Id == userId));
        }

        public IEnumerable<FileMetadata> SearchFilesByName(string name, int userId)
        {
            var files = _context.Files.Where(f => (f.Name.ToLower() + f.Extension.ToLower())
                .Contains(name.ToLower()));

            return files.Where(f => f.Accessability == FileAccessabilities.Public ||
                 f.User.Id == userId || f.PermittedUsers.Any(u => u.Id == userId));
        }

        public async Task<IEnumerable<FileMetadata>> SearchFilesByNameAsync(string name)
        {
            return await _context.Files.Where(f => f.Accessability == FileAccessabilities.Public &&
                 (f.Name.ToLower() + f.Extension.ToLower())
                    .Contains(name.ToLower()))
                    .ToListAsync();
        }

        public async Task<IEnumerable<FileMetadata>> GetFilesAsync(int? userId)
        {
            if (userId == null)
                return await _context.Files.Where(f => f.Accessability == FileAccessabilities.Public).ToListAsync();

            return _context.Files.Where(f => f.Accessability == FileAccessabilities.Public || f.User.Id == userId ||
                f.PermittedUsers.Any(u => u.Id == userId)); ;
        }

        public async Task DeleteFileAsync(FileMetadata file)
        {
            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFileAsync(FileMetadata file)
        {
            _context.Files.Update(file);
            await _context.SaveChangesAsync();
        }

    }
}

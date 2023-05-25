using Domain.Enums;
using Domain.Models;

namespace Repository.Repositories.Interfaces
{
    public interface IFileMetadataRepository
    {
        public Task AddFile(FileMetadata file);
        public Task<FileMetadata> GetFileAsync(string fileId);
        Task<IEnumerable<FileMetadata>> GetUserFilesAsync(int userId);
        Task<IEnumerable<FileMetadata>> GetUserFilesAsync(int userId, int requesterId,
            FileAccessabilities? accessability = null, string? searchTerm = null);
        Task<bool> CheckPermission(int userId, string fileId);
        IEnumerable<FileMetadata> SearchFilesByName(string name, int userId);
        Task<IEnumerable<FileMetadata>> SearchFilesByNameAsync(string name);
        Task<IEnumerable<FileMetadata>> GetFilesAsync(int? userId);
        Task DeleteFileAsync(FileMetadata file);
        Task UpdateFileAsync(FileMetadata file);
    }
}

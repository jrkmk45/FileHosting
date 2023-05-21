using Domain.Models;

namespace Repository.Repositories.Interfaces
{
    public interface IFileMetadataRepository
    {
        public Task AddFileMetadataAsync(FileMetadata file);
        public Task<FileMetadata> GetFileAsync(string fileId);
        Task<IEnumerable<FileMetadata>> GetUserFilesMetadataAsync(int userId);
        Task<IEnumerable<FileMetadata>> GetUserFilesMetadataAsync(int userId, int ownerId);
        Task<bool> CheckPermission(int userId, string fileId);
        IEnumerable<FileMetadata> SearchFilesByName(string name, int userId);
        Task<IEnumerable<FileMetadata>> SearchFilesByNameAsync(string name);
        Task<IEnumerable<FileMetadata>> GetFilesAsync(int? userId);
        Task DeleteFileAsync(FileMetadata file);
    }
}

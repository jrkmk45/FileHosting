using Domain.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Services.Dtos.FileMetadata;

namespace Services.Services.Interfaces
{
    public interface IFileService
    {
        Task<FileMetadataDTOBase> UploadFileAsync(MultipartReader reader, int userId, bool isPrivate);
        Task<Stream> GetFileStreamAsync(string fileId);
        Task<Stream> GetFileStreamAsync(string fileId, int requesterId);
        Task<FileMetadataDTOBase> GetFileAsync(string fileId);
        Task<FileMetadataDTOBase> GetFileAsync(string fileId, int requesterId);
        Task<IEnumerable<FileMetadataDTOBase>> GetUserPublicFilesAsync(int userId);
        Task<IEnumerable<FileMetadataDTOBase>> GetUserFilesAsync(int userId, int requesterId,
            FileAccessabilities? accessabilities, string? search);
        IEnumerable<FileMetadataDTOBase> SearchFilesByName(string name, int requesterId);
        Task<IEnumerable<FileMetadataDTOBase>> SearchFilesByNameAsync(string name);
        Task<IEnumerable<FileMetadataDTOBase>> GetFilesAsync(int? userId = null);
        Task DeleteFilesAsync(string[] fileIds, int userId);
        Task UpdateFileAsync(string id, UpdateFileDTO changedFile, int userId);
        Task<bool> IsPrivateFileAccessibleAsync(int userId, string fileId);
        Task<string> GetFileDownloadLinkAsync(string fileId, int? userId);
    }
}

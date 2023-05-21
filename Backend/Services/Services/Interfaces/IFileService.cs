using Domain.Models;
using Microsoft.AspNetCore.WebUtilities;
using Services.Dtos.FileMetadata;

namespace Services.Services.Interfaces
{
    public interface IFileService
    {
        Task<FileMetadataDTOBase> UploadFileAsync(MultipartReader reader, int userId, bool isPrivate);
        Task<Stream> GetFileStreamAsync(string fileId);
        Task<Stream> GetFileStreamAsync(string fileId, int requesterId);
        Task<FileMetadataDTOBase> GetFileMetadataAsync(string fileId);
        Task<FileMetadataDTOBase> GetFileMetadataAsync(string fileId, int requesterId);
        Task<IEnumerable<FileMetadataDTOBase>> GetUserPublicFilesAsync(int userId);
        Task<IEnumerable<FileMetadataDTOBase>> GetUserFilesAsync(int userId, int requesterId);
        IEnumerable<FileMetadataDTOBase> SearchFilesByName(string name, int requesterId);
        Task<IEnumerable<FileMetadataDTOBase>> SearchFilesByNameAsync(string name);
        Task<IEnumerable<FileMetadataDTOBase>> GetFilesAsync(int? userId);
        Task DeleteFilesAsync(string[] fileIds, int userId);
    }
}

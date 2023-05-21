using Services.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Repository.Repositories.Interfaces;
using shortid;
using Domain.Models;
using Domain.Constants;
using shortid.Configuration;
using Services.Dtos.FileMetadata;
using AutoMapper;
using Domain.Exceptions;
using System.IO;

namespace Services.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IFileMetadataRepository _fileMetedataRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public FileService(IUserRepository userRepository,
            IFileMetadataRepository fileMetadataRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _fileMetedataRepository = fileMetadataRepository;
            _mapper = mapper;
        }

        public async Task<FileMetadataDTOBase> GetFileMetadataAsync(string fileId)
        {
            var fileMetadata = await _fileMetedataRepository.GetFileAsync(fileId);
            if (fileMetadata == null)
                throw new ResourceNotFoundException($"No file metadata with id: {fileId}");

            if (fileMetadata.isPrivate)
                throw new UnauthorizedAccessException("Authorize to access this resource");

            return _mapper.Map<FileMetadataDTOBase>(fileMetadata);
        }

        public async Task<FileMetadataDTOBase> GetFileMetadataAsync(string fileId, int requesterId)
        {
            var fileMetadata = await _fileMetedataRepository.GetFileAsync(fileId);

            if (fileMetadata == null)
                throw new ResourceNotFoundException($"No file metadata with id: {fileId}");

            if (!fileMetadata.isPrivate || fileMetadata.UserId == requesterId)
                return _mapper.Map<FileMetadataDTOBase>(fileMetadata);

            if (await _fileMetedataRepository.CheckPermission(requesterId, fileId))
                return _mapper.Map<FileMetadataDTOBase>(fileMetadata);

            throw new ForbiddenResourceException("No access to this file!");
        }

        public async Task<Stream> GetFileStreamAsync(string fileId, int requesterId)
        {
            var fileMetadata = await _fileMetedataRepository.GetFileAsync(fileId);

            if (fileMetadata.isPrivate && fileMetadata.User.Id != requesterId
                && await _fileMetedataRepository.CheckPermission(requesterId, fileId))
            {
                throw new ForbiddenResourceException("No access to download this file!");
            }

            return GetFileStream(fileMetadata);
        }

        public async Task<Stream> GetFileStreamAsync(string fileId)
        {
            var fileMetadata = await _fileMetedataRepository.GetFileAsync(fileId);

            if (fileMetadata.isPrivate)
                throw new UnauthorizedException("No access to download this file!");

            return GetFileStream(fileMetadata);
        }

        private FileStream GetFileStream(FileMetadata fileMetadata)
        {
            var filePath = GetFilePath(fileMetadata);
            return new FileStream(filePath, FileMode.Open);
        }
        
        public async Task<IEnumerable<FileMetadataDTOBase>> GetUserPublicFilesAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new Exception($"No user with id: {userId}");

            var filesMetadata = await _fileMetedataRepository.GetUserFilesMetadataAsync(userId);
            return _mapper.Map<IEnumerable<FileMetadataDTOBase>>(filesMetadata);
        }

        public async Task<FileMetadataDTOBase> UploadFileAsync(MultipartReader reader, int userId, bool isPrivate)
        {
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                string fileId = null;

                if (hasContentDispositionHeader)
                {
                    if (!MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                    {
                        throw new Exception("No content disposition header");
                    }
                    else
                    {
                        var trustedFileNameForFileStorage = Path.GetRandomFileName();

                        var dirName = userId.ToString();
                        if (!Directory.Exists("UserDirectories/" + dirName))
                            Directory.CreateDirectory("UserDirectories/" + dirName);

                        using (var targetStream = File.Create(
                            Path.Combine($"UserDirectories/{dirName}", trustedFileNameForFileStorage)))
                        {
                            await section.Body.CopyToAsync(targetStream);
                        }

                        //    var fileName = System.Net.WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                        var fileName = contentDisposition.FileName.Value;

                        var idGenerationOptions = new GenerationOptions(length: 9);
                        fileId = ShortId.Generate(idGenerationOptions);

                        var fileMetadata = new FileMetadata
                        {
                            Id = fileId,
                            Name = Path.GetFileNameWithoutExtension(fileName),
                            Extension = Path.GetExtension(fileName),
                            StorageName = trustedFileNameForFileStorage,
                            Size = section.Body.Length,
                            UserId = userId,
                            CreatedDate = DateTime.Now,
                            isPrivate = isPrivate
                        };
                        await _fileMetedataRepository.AddFileMetadataAsync(fileMetadata);
                    }
                }

                section = await reader.ReadNextSectionAsync();

                var result = await _fileMetedataRepository.GetFileAsync(fileId);
                return _mapper.Map<FileMetadataDTOBase>(result);
            }

            return null;
        }

        public async Task<IEnumerable<FileMetadataDTOBase>> GetUserFilesAsync(int userId, int requesterId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new Exception($"No user with id: {userId}");


            var filesMetadata = await _fileMetedataRepository.GetUserFilesMetadataAsync(userId, requesterId);
            return _mapper.Map<IEnumerable<FileMetadataDTOBase>>(filesMetadata);
        }


        public IEnumerable<FileMetadataDTOBase> SearchFilesByName(string name, int requesterId)
        {
            var files = _fileMetedataRepository.SearchFilesByName(name, requesterId);
            return _mapper.Map<IEnumerable<FileMetadataDTOBase>>(files);
        }

        public async Task<IEnumerable<FileMetadataDTOBase>> SearchFilesByNameAsync(string name)
        {
            var files = await _fileMetedataRepository.SearchFilesByNameAsync(name);
            return _mapper.Map<IEnumerable<FileMetadataDTOBase>>(files);
        }

        public async Task<IEnumerable<FileMetadataDTOBase>> GetFilesAsync(int? userId)
        {
            var files = await _fileMetedataRepository.GetFilesAsync(userId);
            return _mapper.Map<IEnumerable<FileMetadataDTOBase>>(files);
        }

        public async Task DeleteFilesAsync(string[] fileIds, int userId)
        {
            foreach (var fileId in fileIds)
            {
                var file = await _fileMetedataRepository.GetFileAsync(fileId);

                if (file.UserId != userId)
                    throw new ForbiddenResourceException("No permission!");

                var filePath = GetFilePath(file);
                var deletionTask = Task.Run(() => File.Delete(filePath));

                await _fileMetedataRepository.DeleteFileAsync(file);
            }
        }

        private string GetFilePath(FileMetadata fileMetadata)
        {
            return $"{FilePaths.UserDirectories}/{fileMetadata.UserId}/{fileMetadata.StorageName}";
        }

        public static class MultipartRequestHelper
        {
            // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
            // The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
            public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
            {
                var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

                if (string.IsNullOrWhiteSpace(boundary))
                {
                    throw new InvalidDataException("No content type: boundary");
                }

                if (boundary.Length > lengthLimit)
                {
                    throw new InvalidDataException(
                        $"Multipart boundary length limit {lengthLimit} exceeded.");
                }

                return boundary;
            }

            public static bool IsMultipartContentType(string contentType)
            {
                return !string.IsNullOrEmpty(contentType) &&
                    contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
            }

            public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
            {
                // Content-Disposition: form-data; name="key";
                return contentDisposition != null &&
                    contentDisposition.DispositionType.Equals("form-data") &&
                    string.IsNullOrEmpty(contentDisposition.FileNameStar.Value) &&
                    string.IsNullOrEmpty(contentDisposition.FileName.Value) ;
            }

            public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
            {
                // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
                return contentDisposition != null &&
                    contentDisposition.DispositionType.Equals("form-data") &&
                    (!string.IsNullOrEmpty(contentDisposition.FileName.Value) ||
                    !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
            }
        }
    }
}

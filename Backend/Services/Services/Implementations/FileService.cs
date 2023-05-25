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
using Domain.Enums;
using Services.Utils;

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

        public async Task<FileMetadataDTOBase> GetFileAsync(string fileId)
        {
            var fileMetadata = await _fileMetedataRepository.GetFileAsync(fileId);
            if (fileMetadata == null)
                throw new ResourceNotFoundException($"No file metadata with id: {fileId}");

            if (fileMetadata.Accessability == FileAccessabilities.Private)
                throw new UnauthorizedAccessException("Authorize to access this resource");

            return _mapper.Map<FileMetadataDTOBase>(fileMetadata);
        }

        public async Task<FileMetadataDTOBase> GetFileAsync(string fileId, int requesterId)
        {
            var fileMetadata = await _fileMetedataRepository.GetFileAsync(fileId);

            if (fileMetadata == null)
                throw new ResourceNotFoundException($"No file metadata with id: {fileId}");

            if (fileMetadata.UserId == requesterId)
                return _mapper.Map<FileFullDTO>(fileMetadata);

            if (fileMetadata.Accessability != FileAccessabilities.Private)
                return _mapper.Map<FileMetadataDTOBase>(fileMetadata);

            if (await _fileMetedataRepository.CheckPermission(requesterId, fileId))
                return _mapper.Map<FileMetadataDTOBase>(fileMetadata);

            throw new ForbiddenResourceException("No access to this file!");
        }

        public async Task<Stream> GetFileStreamAsync(string fileId, int requesterId)
        {
            var fileMetadata = await _fileMetedataRepository.GetFileAsync(fileId);

            if (fileMetadata.Accessability == FileAccessabilities.Private && fileMetadata.User.Id != requesterId
                && !await _fileMetedataRepository.CheckPermission(requesterId, fileId))
            {
                throw new ForbiddenResourceException("No access to download this file!");
            }

            return GetFileStream(fileMetadata);
        }

        public async Task<Stream> GetFileStreamAsync(string fileId)
        {
            var fileMetadata = await _fileMetedataRepository.GetFileAsync(fileId);

            if (fileMetadata.Accessability == FileAccessabilities.Private)
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

            var filesMetadata = await _fileMetedataRepository.GetUserFilesAsync(userId);
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

                        string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                        if (!Directory.Exists("${FilePaths.UserDirectories}/" + dirName))
                            Directory.CreateDirectory($"{FilePaths.UserDirectories}/" + dirName);

                        using (var targetStream = File.Create(
                            Path.Combine($"{FilePaths.UserDirectories}/{dirName}", trustedFileNameForFileStorage)))
                        {
                            await section.Body.CopyToAsync(targetStream);
                        }
                        
                        var fileName = System.Net.WebUtility.HtmlEncode(contentDisposition.FileName.Value);

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
                            Accessability = isPrivate ? FileAccessabilities.Private : FileAccessabilities.Public,
                        };
                        await _fileMetedataRepository.AddFile(fileMetadata);
                    }
                }

                section = await reader.ReadNextSectionAsync();

                var result = await _fileMetedataRepository.GetFileAsync(fileId);
                return _mapper.Map<FileMetadataDTOBase>(result);
            }

            return null;
        }

        public async Task<IEnumerable<FileMetadataDTOBase>> GetUserFilesAsync(int userId, int requesterId, 
            FileAccessabilities? accessability = null, string? search = null)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new Exception($"No user with id: {userId}");


            var filesMetadata = await _fileMetedataRepository.GetUserFilesAsync(userId, requesterId, accessability, search);
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

        public async Task<IEnumerable<FileMetadataDTOBase>> GetFilesAsync(int? userId = null)
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
                Task.Run(() => File.Delete(filePath));

                await _fileMetedataRepository.DeleteFileAsync(file);
            }
        }

        public async Task UpdateFileAsync(string id, UpdateFileDTO changedFile, int userId)
        {
            var file = await _fileMetedataRepository.GetFileAsync(id);

            if (file == null)
                throw new ResourceNotFoundException("No file with id" + id);

            if (file.UserId != userId)
                throw new ForbiddenResourceException($"{userId} is not owner of {id}");

            _mapper.Map(changedFile, file);
            
            for (int i=0; i < file.PermittedUsers.Count; i++)
            {
                file.PermittedUsers[i] = await _userRepository.GetUserByIdAsync(file.PermittedUsers[i].Id);
            }
           
            await _fileMetedataRepository.UpdateFileAsync(file);
        }

        private string GetFilePath(FileMetadata fileMetadata)
        {
            return $"{FilePaths.UserDirectories}/{fileMetadata.UserId}/{fileMetadata.StorageName}";
        }

        public Task<bool> IsPrivateFileAccessibleAsync(int userId, string fileId)
        {
            return _fileMetedataRepository.CheckPermission(userId, fileId);
        }

        public async Task<string> GetFileDownloadLinkAsync(string fileId, int? userId = null)
        {
            var file = await _fileMetedataRepository.GetFileAsync(fileId);
            var tokenExpirationDate = DateTime.UtcNow.AddMinutes(3);
            if (file.Accessability != FileAccessabilities.Private)
            {
                var downloadToken = DownloadTokenManager.GenerateFileDownloadingToken(userId, fileId, tokenExpirationDate);
                return $"{await IPGetter.GetPublicIPAsync()}/api/files/download?token={downloadToken}";
            }
            
            if (userId == null)
                throw new UnauthorizedException("Downloading this file requires authorization");

            if (!await _fileMetedataRepository.CheckPermission((int)userId, fileId))
                throw new ForbiddenResourceException($"No permission to download tihd file");

            var token = DownloadTokenManager.GenerateFileDownloadingToken(userId, fileId, tokenExpirationDate);
            return $"{await IPGetter.GetPublicIPAsync()}/api/files/download?token={token}";
        }

        public static class MultipartRequestHelper
        {
            public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
            {
                return contentDisposition != null &&
                    contentDisposition.DispositionType.Equals("form-data") &&
                    (!string.IsNullOrEmpty(contentDisposition.FileName.Value) ||
                    !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
            }
        }
    }
}

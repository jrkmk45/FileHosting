
using Domain.Exceptions;
using Domain.Models;
using dotenv.net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Services.Dtos.FileMetadata;
using Services.Dtos.User;
using Services.Services.Interfaces;
using Services.Utils;
using System.Security.Claims;

namespace ForumAPI.Controllers
{
    [DisableRequestSizeLimit]
    [Route("api/files/")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        public FilesController(IFileService fileService, IUserService userService)
        {
            _fileService = fileService;
            _userService = userService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FileMetadataDTOBase>>> SearchFiles(string name)
        {
            if (!User.Identity.IsAuthenticated)
                return Ok(await _fileService.SearchFilesByNameAsync(name));

            var requesterId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(_fileService.SearchFilesByName(name, requesterId));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileMetadataDTOBase>>> GetFiles()
        {
            if (!User.Identity.IsAuthenticated)
                return Ok(await _fileService.GetFilesAsync());

            var requesterId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _fileService.GetFilesAsync(requesterId));
        }

        [HttpDelete, Authorize]
        public async Task<ActionResult> DeleteFiles([FromBody] string[] fileIds)
        {
            try
            {
                var requesterId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _fileService.DeleteFilesAsync(fileIds, requesterId);
                return NoContent();
            } catch (ForbiddenResourceException)
            {
                return Forbid();
            }
        }
        
        [HttpGet("{fileId}")]
        public async Task<ActionResult<FileMetadataDTOBase>> GetFile(string fileId)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return Ok(await _fileService.GetFileAsync(fileId));

                var requesterId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(await _fileService.GetFileAsync(fileId, requesterId));
            }
            catch (ForbiddenResourceException ex)
            {
                return Forbid();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{fileId}/download-link")]
        public async Task<ActionResult<string>> GetFileDownloadLink(string fileId)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    return Ok(await _fileService.GetFileDownloadLinkAsync(fileId, userId));
                }
                return Ok(await _fileService.GetFileDownloadLinkAsync(fileId, null));
            }
            catch (ForbiddenResourceException)
            {
                return Forbid();
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(ex.Message);
            }

        }


        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile(string? token)
        {
            try
            {
                var enviroment = DotEnv.Read();
                var signingKey = enviroment["JWT_KEY"];
                var principal = DownloadTokenManager.ValidateToken(token, signingKey);

                var fileId = principal.FindFirst("fileId")?.Value!;

                FileMetadataDTOBase fileMetadata;
                Stream fileStream;
                if (int.TryParse(principal.FindFirst("userId")?.Value!, out var userId))
                {
                    fileMetadata = await _fileService.GetFileAsync(fileId, userId);
                    fileStream = await _fileService.GetFileStreamAsync(fileId, userId);
                }
                else
                {
                    fileMetadata = await _fileService.GetFileAsync(fileId);
                    fileStream = await _fileService.GetFileStreamAsync(fileId);
                }
                return File(fileStream, "application/octet-stream", fileMetadata.FullName);

            }
            catch (SecurityTokenValidationException)
            {
                return Unauthorized();
            } 
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<FileMetadataDTOBase>> UploadFiles()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
                return BadRequest($"Expected a multipart content type, but got {Request.ContentType}");

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), 1000);
            
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));


            if (!Request.Headers.TryGetValue("isPrivate", out var isPrivate))
            {
                return BadRequest("Missing isPrivate header");
            }

            var fileMetadata = await _fileService.UploadFileAsync(reader, userId, Convert.ToBoolean(isPrivate));

            return Created(nameof(FilesController), fileMetadata);
           
        }

        [HttpPatch("{fileId}"), Authorize]
        public async Task<ActionResult> UpdateFile(string fileId, [FromBody]UpdateFileDTO updateFile)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _fileService.UpdateFileAsync(fileId, updateFile, userId);
                return NoContent();
            } catch (ForbiddenResourceException)
            {
                return Forbid();
            }
            catch (ResourceNotFoundException) 
            {
                return NotFound("No file with id" + fileId);
            }
        }

        [HttpGet("{fileId}/users/non-permitted"), Authorize]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAccessedUsersByFile(string fileId, string? search)
        {
            try
            {
                var requesterId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(await _userService.GetNonAccessedUsersByFile(fileId, search, requesterId));
            }
            catch (ForbiddenResourceException)
            {
                return Forbid();
            }
        }

        public static class MultipartRequestHelper
        {
            public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
            {
                var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

                if (string.IsNullOrWhiteSpace(boundary))
                {
                    throw new InvalidDataException("Missing content-type boundary.");
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
                return !string.IsNullOrEmpty(contentType)
                       && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }
        
    }
}

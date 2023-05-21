using Domain.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Services.Dtos.FileMetadata;
using Services.Services.Interfaces;
using System.Security.Claims;

namespace ForumAPI.Controllers
{
    [DisableRequestSizeLimit]
    [Route("api/files/")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FilesController(IFileService fileService) 
        { 
            _fileService = fileService;
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
                return Ok(await _fileService.GetFilesAsync(null));

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
                    return Ok(await _fileService.GetFileMetadataAsync(fileId));

                var requesterId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(await _fileService.GetFileMetadataAsync(fileId, requesterId));
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
        
        [HttpGet("{fileId}/download")]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            try
            {
                Stream fileStream;
                if (!User.Identity.IsAuthenticated)
                {
                    fileStream = await _fileService.GetFileStreamAsync(fileId);
                }
                else
                {
                    var requesterId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    fileStream = await _fileService.GetFileStreamAsync(fileId, requesterId);
                }

                var fileMetadata = await _fileService.GetFileMetadataAsync(fileId);
                return File(fileStream, "application/octet-stream", fileMetadata.FullName);
            } catch (ForbiddenResourceException)
            {
                return Forbid();
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
        
        public static class MultipartRequestHelper
        {
            // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
            // The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
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

using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services.Dtos.FileMetadata;
using Services.Dtos.User;
using Services.Services.Interfaces;
using System.Security.Claims;

namespace ForumAPI.Controllers
{
    [EnableCors]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        public UserController(IUserService userService,
            IFileService fileService)
        {
            _userService = userService;
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<ActionResult<UserManagerResponseDTO>> Register([FromBody] RegisterUserDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid properties");

            var result = await _userService.AddUserAsync(model);

            if (result.IsSuccess)
                return Ok(result);

            return Unauthorized(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserManagerResponseDTO>> Login([FromBody] LoginUserDTO loginData)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid properties");

            var result = await _userService.LoginUserAsync(loginData);

            if (result.IsSuccess)
                return Ok(result);

            return Unauthorized(result);
        }


        [HttpGet("me"), Authorize]
        public async Task<ActionResult<UserDTO>> GetUserByToken()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _userService.GetUserByIdAsync(userId));
        }

        [HttpPut("me"), Authorize]
        public async Task<ActionResult> UpdateUser([FromForm] UpdateUserDTO user)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _userService.UpdateUserAsync(userId, user);
                return NoContent();
            } catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{userId:int}/files")]
        public async Task<ActionResult<IEnumerable<FileMetadataDTOBase>>> GetUserFiles(int userId, FileAccessabilities? access, string? search)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    if (access != null && access != FileAccessabilities.Public)
                        return Unauthorized("User should be authorized to filter by this access");

                    return Ok(await _fileService.GetUserPublicFilesAsync(userId));
                }

                 var requesterId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                 return Ok(await _fileService.GetUserFilesAsync(userId, requesterId, access, search));
                
            } catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDTO>> GetUser(int userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers(string? searchTerm)
        {
            if (searchTerm == null)
                return Ok(await _userService.GetUsersAsync());

            return Ok(await _userService.SearchUsersByUserNameAsync(searchTerm));
        }
        
        
    }
}

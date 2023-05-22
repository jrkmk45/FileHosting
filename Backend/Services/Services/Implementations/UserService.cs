using AutoMapper;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Repositories.Interfaces;
using Services.Dtos.User;
using Services.Services.Inerfaces;
using Services.Services.Interfaces;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotenv.net;
using Services.Utils;

namespace Services.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IImageUploadService _imageUploadService;

        private const string InvalidCredentialsMessage = "Невалідні дані для логіну";
        private const string UnableToCreateUserMessage = "Реєстрація неможлива";
        public UserService(UserManager<User> userManager,
            IMapper mapper,
            IUserRepository userRepository,
            IImageUploadService imageUploadService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userRepository = userRepository;
            _imageUploadService = imageUploadService;
        }

        public async Task<UserManagerResponseDTO> LoginUserAsync(LoginUserDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                return new UserManagerResponseDTO
                {
                    Message = InvalidCredentialsMessage,
                    IsSuccess = false
                };
            }

            var authResult = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!authResult)
            {
                return new UserManagerResponseDTO
                {
                    Message = InvalidCredentialsMessage,
                    IsSuccess = false
                };
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var enviroment = DotEnv.Read();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(enviroment["JWT_KEY"]));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponseDTO
            {
                Token = stringToken,
                Message = "Успішний вхід",
                IsSuccess = true,
                TokenExpirationDate = token.ValidTo,
            };
        }

        public async Task<UserManagerResponseDTO> AddUserAsync(RegisterUserDTO model)
        {
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                return new UserManagerResponseDTO
                {
                    Message = UnableToCreateUserMessage,
                    IsSuccess = false,
                    Errors = new List<string> { "Цей логін зайнятий" }
                };
            }

            if (model.UserName.Length > 25)
            {
                return new UserManagerResponseDTO
                {
                    Message = UnableToCreateUserMessage,
                    IsSuccess = false,
                    Errors = new List<string> { "Максимальна довжина логіну - 25 символів" }
                };
            }

            var identityUser = _mapper.Map<User>(model);

            var result = await _userManager.CreateAsync(identityUser, model.Password);


            if (result.Succeeded)
            {
                return new UserManagerResponseDTO
                {
                    Message = "Успішна реєстрація",
                    IsSuccess = true
                };
            }

            return new UserManagerResponseDTO
            {
                Message = UnableToCreateUserMessage,
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var mappedUser = _mapper.Map<UserDTO>(user);
            return mappedUser;
        }

        public async Task UpdateUserAsync(int id, UpdateUserDTO updateUser)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new ResourceNotFoundException("This user does not exists!");
            
            if (updateUser.ProfilePicture != null)
                user.ProfilePicture = await _imageUploadService.UploadImageAsync(updateUser.ProfilePicture, FilePaths.AvatarsPaths);
            
            var mappedUser = _mapper.Map<UserDTO>(user);
            await _userRepository.UpdateUserAsync(user);
        }

    }
}

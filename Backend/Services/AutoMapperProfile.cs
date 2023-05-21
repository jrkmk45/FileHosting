using AutoMapper;
using Domain.Constants;
using Domain.Models;
using Services.Dtos.FileMetadata;
using Services.Dtos.User;

namespace ForumAPI
{
    public class AutoMapperProfile : Profile
    {
        private readonly string ImagesUrl = $"http://localhost/{FilePaths.AvatarsPaths}";


        public AutoMapperProfile()
        {
            CreateMap<RegisterUserDTO, User>();

            CreateMap<User, UserDTO>()
                .ForMember(opt => opt.ProfilePicture, opt =>
                {
                    opt.PreCondition(e => e.ProfilePicture != null);
                    opt.MapFrom(e => ImagesUrl + e.ProfilePicture);
                });
            //        .ForMember(opt => opt.Id, opt => opt.MapFrom(e => e.Id))
            //        .ForMember(opt => opt.UserName, opt => opt.MapFrom(e => e.UserName))

            CreateMap<FileMetadata, FileMetadataDTOBase>()
                .ForMember(opt => opt.FullName, opt => opt.MapFrom(e => $"{e.Name}{e.Extension}"));

        }
    }
}

using AutoMapper;
using Domain.Constants;
using Domain.Models;
using Services.Dtos.FileMetadata;
using Services.Dtos.User;
using Services.Utils;

namespace ForumAPI
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            var Ip = IPGetter.GetPublicIPAsync().Result;
            var ImagesUrl = $"{Ip}/{FilePaths.AvatarsPaths}";

            CreateMap<RegisterUserDTO, User>();

            CreateMap<User, UserDTO>()
                .ForMember(opt => opt.ProfilePicture, opt =>
                {
                    opt.PreCondition(e => e.ProfilePicture != null);
                    opt.MapFrom(e => ImagesUrl + e.ProfilePicture);
                });

            CreateMap<FileMetadata, FileMetadataDTOBase>()
                .ForMember(opt => opt.FullName, opt => opt.MapFrom(e => $"{e.Name}{e.Extension}"));

        }
    }
}

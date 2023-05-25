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
            var ImagesUrl = $"http://{Ip}/{FilePaths.AvatarsPaths}";

            CreateMap<RegisterUserDTO, User>();

            CreateMap<User, UserDTO>()
                .ForMember(opt => opt.ProfilePicture, opt =>
                {
                    opt.PreCondition(e => e.ProfilePicture != null);
                    opt.MapFrom(e => ImagesUrl + e.ProfilePicture);
                });

            CreateMap<FileMetadata, FileMetadataDTOBase>()
                .ForMember(opt => opt.FullName, opt => opt.MapFrom(e => $"{e.Name}{e.Extension}"))
                .ForMember(opt => opt.Accessability, opt => opt.MapFrom(e => (int)e.Accessability))
                .ForMember(opt => opt.Owner, opt => opt.MapFrom(e => e.User))
                .IncludeAllDerived();

            CreateMap<UpdateFileDTO, FileMetadata>()
                .ForMember(opt => opt.Accessability, opt =>
                {
                    opt.PreCondition(e => e.Accesability != null);
                    opt.MapFrom(e => e.Accesability);
                })
                .ForMember(opt => opt.Name, opt =>
                {
                    opt.PreCondition(e => e.Name != null);
                    opt.MapFrom(e => e.Name);
                })
                .ForMember(opt => opt.PermittedUsers, opt =>
                {
                    opt.PreCondition(e => e.PermittedUsers != null);
                    opt.MapFrom(e => e.PermittedUsers);
                });

            CreateMap<int, User>().ForMember(opt => opt.Id, opt => opt.MapFrom(e => e));

            CreateMap<FileMetadata, FileFullDTO>();
        }
    }
}

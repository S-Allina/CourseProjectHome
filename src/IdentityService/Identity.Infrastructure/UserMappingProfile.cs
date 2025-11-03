using AutoMapper;
using Identity.Application.Dto;
using Identity.Domain.Entity;

namespace Identity.Infrastructure
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, CurrentUserDto>().ReverseMap();
            //CreateMap<UserUpdateRequestDto, ApplicationUser>().ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            //    .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            //    .ForMember(dest => dest.RefreshTokenExpiryTime, opt => opt.Ignore())
            //    .ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();
            CreateMap<UserRegistrationRequestDto, ApplicationUser>();
        }
    }
}

using AutoMapper;
using Identity.Application.Dto;
using Identity.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, CurrentUserDto>().ForMember(dest => dest.AccessToken, opt => opt.Ignore()).ReverseMap();
            //CreateMap<UserUpdateRequestDto, ApplicationUser>().ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            //    .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            //    .ForMember(dest => dest.RefreshTokenExpiryTime, opt => opt.Ignore())
            //    .ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();
            CreateMap<UserRegistrationRequestDto, ApplicationUser>();
        }
    }
}

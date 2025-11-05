using AutoMapper;
using Main.Application.Dtos.Common;
using Main.Domain.entities.common;

namespace Main.Application.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, CreateUserRequest>().ReverseMap();
        }
    }
}

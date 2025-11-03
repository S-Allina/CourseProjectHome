using AutoMapper;
using Main.Application.Dtos.Common;
using Main.Application.Dtos.Items.Index;
using Main.Domain.entities.common;
using Main.Domain.entities.item;

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

using AutoMapper;
using Main.Application.Dtos.Common;
using Main.Domain.entities.common;

namespace Main.Application.Mapper
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<ChatMessage, ChatMessageDto>().ReverseMap();
        }
    }
}

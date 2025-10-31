using AutoMapper;
using Main.Application.Dtos.Items.Index;
using Main.Domain.entities.common;
using Main.Domain.entities.item;

namespace Main.Application.Mapper
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<Item, ItemDto>()
                .ForMember(dest => dest.FieldValues, opt => opt.MapFrom(src => src.FieldValues));

            CreateMap<ItemDto, Item>()
                .ForMember(dest => dest.FieldValues, opt => opt.MapFrom(src => src.FieldValues))
                                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<ItemFieldValue, ItemFieldValueDto>()
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.InventoryField.Name))
                .ForMember(dest => dest.FieldType, opt => opt.MapFrom(src => src.InventoryField.FieldType)).ReverseMap();
        }
    }
}

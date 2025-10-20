using AutoMapper;
using Main.Application.Dtos;
using Main.Domain.entities.item;
using Main.Domain.enums.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            CreateMap<ItemFieldValue, ItemFieldValueDto>()
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.InventoryField.Name))
                .ForMember(dest => dest.FieldType, opt => opt.MapFrom(src => src.InventoryField.FieldType)).ReverseMap();
        }

        
    }
}

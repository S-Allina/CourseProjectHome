using AutoMapper;
using Main.Application.Dtos;
using Main.Domain.entities.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Mapper
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            CreateMap<CreateInventoryDto, Inventory>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Trim() : null))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CurrentSequence, opt => opt.MapFrom(_ => 1))
                .ForMember(dest => dest.Fields, opt => opt.Ignore()) // Обрабатываем отдельно в сервисе
                .ForMember(dest => dest.Tags, opt => opt.Ignore())   // Обрабатываем отдельно в сервисе
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore());

            CreateMap<CreateInventoryFieldDto, InventoryField>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Trim() : null))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Inventory → InventoryDto
            CreateMap<Inventory, InventoryDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag.Name).ToList()))
                .ForMember(dest => dest.Fields, opt => opt.MapFrom(src => src.Fields));

            // ✅ ИСПРАВЛЕННЫЙ МАППИНГ: InventoryDto → Inventory с поддержкой Fields
            CreateMap<InventoryDto, Inventory>()
                .ForMember(dest => dest.Fields, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    // Маппим Fields из DTO в Entity
                    if (src.Fields == null) return new List<InventoryField>();

                    return src.Fields.Select(fieldDto =>
                    {
                        var field = context.Mapper.Map<InventoryField>(fieldDto);
                        field.InventoryId = src.Id; // Устанавливаем связь
                        return field;
                    }).ToList();
                }))
                .ForMember(dest => dest.Tags, opt => opt.Ignore()) // Tags обрабатываем отдельно
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.AccessList, opt => opt.Ignore());

            CreateMap<UpdateInventoryBasicInfoDto, Inventory>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.Fields, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.AccessList, opt => opt.Ignore());

            // ✅ ДОБАВЛЯЕМ маппинг для InventoryFieldDto → InventoryField
            CreateMap<InventoryFieldDto, InventoryField>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Trim() : null))
                .ForMember(dest => dest.FieldType, opt => opt.MapFrom(src => src.FieldType))
                .ForMember(dest => dest.OrderIndex, opt => opt.MapFrom(src => src.OrderIndex))
                .ForMember(dest => dest.IsVisibleInTable, opt => opt.MapFrom(src => src.IsVisibleInTable))
                .ForMember(dest => dest.IsRequired, opt => opt.MapFrom(src => src.IsRequired))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryId, opt => opt.Ignore()) // Устанавливается в родительском маппинге
                .ForMember(dest => dest.Inventory, opt => opt.Ignore());

            CreateMap<InventoryField, InventoryFieldDto>().ReverseMap();
        }
    }
}
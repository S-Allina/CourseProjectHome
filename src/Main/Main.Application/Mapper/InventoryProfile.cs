using AutoMapper;
using Main.Application.Dtos.Inventories.Create;
using Main.Application.Dtos.Inventories.Index;
using Main.Application.Dtos.Items.Create;
using Main.Domain.entities.inventory;
using Main.Presentation.MVC.ViewModel;

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
                .ForMember(dest => dest.Fields, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.AccessList, opt => opt.Ignore());

            CreateMap<CreateInventoryFieldDto, InventoryField>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Trim() : null))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Inventory, InventoryTableDto>()
           .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag.Name).ToList()))
           .ForMember(dest => dest.FieldCount, opt => opt.MapFrom(src => src.Fields.Count))
           .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items.Count))
           .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src =>
               $"{src.Owner.FirstName} {src.Owner.LastName}"))
           .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
               src.Category != null ? src.Category.Name : null)).ReverseMap();

            CreateMap<Inventory, InventoryDetailsDto>()
                .IncludeBase<Inventory, InventoryTableDto>()
                .ForMember(dest => dest.Fields, opt => opt.MapFrom(src => src.Fields))
                .ForMember(dest => dest.AccessList, opt => opt.MapFrom(src => src.AccessList)).ReverseMap();

            CreateMap<InventoryFieldDto, InventoryField>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Trim() : null))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryId, opt => opt.Ignore()) 
                .ForMember(dest => dest.Inventory, opt => opt.Ignore());

            CreateMap<Inventory, InventoryFormDto>()
           .ForMember(dest => dest.Version, opt => opt.MapFrom(src => Convert.ToBase64String(src.Version)))
           .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag.Name).ToList()));

            CreateMap<InventoryFormViewModel, InventoryFormDto>().ReverseMap();
            CreateMap<InventoryFormDto, Inventory>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 0))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src =>
                    src.Version != null ? Convert.FromBase64String(src.Version) : new byte[8]))
                .ForMember(dest => dest.Tags, opt => opt.Ignore()) 
                .ForMember(dest => dest.Fields, opt => opt.Ignore())
                .ForMember(dest => dest.AccessList, opt => opt.Ignore()) 
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            CreateMap<CreateInventoryFieldDto, InventoryFieldDto>();
            CreateMap<InventoryField, CreateInventoryFieldDto>();
            CreateMap<InventoryField, InventoryFieldDto>().ReverseMap();
            CreateMap<InventoryAccess, InventoryAccessDto>().ReverseMap();
        }
    }
}
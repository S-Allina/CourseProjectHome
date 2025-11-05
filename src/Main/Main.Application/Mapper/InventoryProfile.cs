using AutoMapper;
using Main.Application.Dtos.Inventories.Create;
using Main.Application.Dtos.Inventories.Index;
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

                .ForMember(dest => dest.AccessList, opt => opt.MapFrom(src => src.AccessList));

            CreateMap<Inventory, InventoryTableDto>()
     .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
         src.Tags != null ? src.Tags.Where(t => t.Tag != null).Select(t => t.Tag.Name).ToList() : new List<string>()))
     .ForMember(dest => dest.FieldCount, opt => opt.MapFrom(src => src.Fields != null ? src.Fields.Count : 0))
     .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items != null ? src.Items.Count : 0))
     .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src =>
         src.Owner != null ? $"{src.Owner.FirstName} {src.Owner.LastName}" : string.Empty))
     .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
         src.Category != null ? src.Category.Name : null));

            CreateMap<Inventory, InventoryDetailsDto>()
                .IncludeBase<Inventory, InventoryTableDto>()
                .ForMember(dest => dest.Fields, opt => opt.MapFrom(src => src.Fields != null ? src.Fields : new List<InventoryField>()))
                .ForMember(dest => dest.AccessList, opt => opt.MapFrom(src => src.AccessList != null ? src.AccessList : new List<InventoryAccess>()))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version));

            CreateMap<InventoryDetailsDto, Inventory>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name != null ? src.Name.Trim() : string.Empty))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Trim() : string.Empty))
                .ForMember(dest => dest.CustomIdFormat, opt => opt.MapFrom(src => src.CustomIdFormat))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))

                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.ChatMessages, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());


            CreateMap<InventoryFieldDto, InventoryField>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Trim() : null))
                .ForMember(dest => dest.Inventory, opt => opt.Ignore());

            CreateMap<Inventory, InventoryFormDto>()
           .ForMember(dest => dest.Version, opt => opt.MapFrom(src => Convert.ToBase64String(src.Version)))
           .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag.Name).ToList()));

            CreateMap<InventoryFormDto, InventoryFormViewModel>()
    .ForMember(dest => dest.Categories, opt => opt.Ignore())
    .ReverseMap();

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
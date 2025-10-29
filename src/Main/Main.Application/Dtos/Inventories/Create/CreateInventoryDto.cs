using Microsoft.AspNetCore.Http;

namespace Main.Application.Dtos.Inventories.Create
{

    public record CreateInventoryDto
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public int? CategoryId { get; init; }
        public string? ImageUrl { get; set; }
        public IFormFile Image { get; init; }
        public bool IsPublic { get; init; }
        public string CustomIdFormat { get; init; }
        public byte[] Version { get; set; }
        public List<string> Tags { get; init; } = new();
        public List<CreateInventoryFieldDto> Fields { get; init; } = new();
        public List<CreateInventoryAccessDto> AccessList { get; set; } = new();
    }
}

using Microsoft.AspNetCore.Http;

namespace Main.Application.Dtos.Inventories.Create
{

    public record CreateInventoryDto
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public int? CategoryId { get; init; }
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; init; }
        public bool IsPublic { get; init; }
        public required string CustomIdFormat { get; init; }
        public byte[]? Version { get; set; }
        public List<string>? Tags { get; init; }
        public List<CreateInventoryFieldDto>? Fields { get; init; }
        public List<CreateInventoryAccessDto>? AccessList { get; set; }
    }
}

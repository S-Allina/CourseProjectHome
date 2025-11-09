using Main.Application.Helpers;
using Main.Domain.enums.inventory;
using Microsoft.AspNetCore.Http;

namespace Main.Application.Dtos.Inventories.Index
{
    public record InventoryTableDto
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string Description { get; init; } = string.Empty;
        public string DescriptionHtml => MarkdownHelper.ConvertToHtml(Description);
        public int? CategoryId { get; init; }
        public string? CategoryName { get; init; }
        public string? OwnerId { get; init; }
        public string? OwnerName { get; init; }
        public string? ImageUrl { get; set; }
        public bool IsPublic { get; init; }
        public int? ItemsCount { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public List<string>? Tags { get; init; }
        public int? FieldCount { get; init; }
        public string? CustomIdFormat { get; init; }
    }

    public record InventoryDetailsDto : InventoryTableDto
    {
        public IFormFile? Image { get; init; }
        public byte[]? Version { get; init; }
        public List<InventoryFieldDto>? Fields { get; init; }
        public List<InventoryAccessDto>? AccessList { get; init; }
    }
}

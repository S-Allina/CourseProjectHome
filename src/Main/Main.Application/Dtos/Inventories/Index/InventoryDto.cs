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
    public record InventoryStatsDto
    {
        public int TotalItems { get; init; }
        public int TotalFields { get; init; }
        public DateTime? OldestItemDate { get; init; }
        public DateTime? NewestItemDate { get; init; }
        public List<FieldStatsDto>? FieldStatistics { get; init; }
    }

    public record FieldStatsDto
    {
        public int FieldId { get; init; }
        public required string FieldName { get; init; }
        public FieldType FieldType { get; init; }

        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public double? AverageValue { get; set; }

        public Dictionary<string, int>? ValueCounts { get; set; }
        public int? UniqueValuesCount { get; set; }

        public int? EmptyValuesCount { get; init; }
        public int? NonEmptyValuesCount { get; init; }
    }

    public record NumericFieldStatsDto
    {
        public int FieldId { get; init; }
        public required string FieldName { get; init; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double AverageValue { get; set; }
        public List<ValueCountDto> ValueDistribution { get; init; } = new();
    }

    public record TextFieldStatsDto
    {
        public int FieldId { get; init; }
        public required string FieldName { get; init; }
        public List<ValueCountDto> TopValues { get; init; } = new();
        public int UniqueValuesCount { get; set; }
    }

    public record ValueCountDto
    {
        public required string Value { get; init; }
        public int Count { get; init; }
    }
}

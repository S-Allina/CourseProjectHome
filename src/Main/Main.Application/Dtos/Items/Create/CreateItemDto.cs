namespace Main.Application.Dtos.Items.Create
{
    public record CreateItemDto
    {
        public int Id { get; set; }
        public string? CreatedById { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public byte[]? Version { get; set; }
        public int InventoryId { get; init; }
        public string? CustomId { get; init; }
        public List<CreateItemFieldValueDto>? FieldValues { get; init; }
    }
}

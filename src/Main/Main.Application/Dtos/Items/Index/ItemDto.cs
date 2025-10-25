namespace Main.Application.Dtos.Items.Index
{
    public record ItemDto
    {
        public int Id { get; init; }
        public int InventoryId { get; init; }
        public string CustomId { get; init; }
        public string CreatedById { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public List<ItemFieldValueDto> FieldValues { get; init; }
    }
}

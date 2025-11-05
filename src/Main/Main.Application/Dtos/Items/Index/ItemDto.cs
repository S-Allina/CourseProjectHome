namespace Main.Application.Dtos.Items.Index
{
    public class ItemDto
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public required string CustomId { get; set; }
        public required string CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public byte[] Version { get; set; }
        public List<ItemFieldValueDto>? FieldValues { get; set; }
    }
}

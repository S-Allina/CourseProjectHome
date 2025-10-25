namespace Main.Application.Dtos.Inventories.Index
{
    public class InventoryAccessDto
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public InventoryFormDto Inventory { get; set; }

        public string UserId { get; set; }
        public string GrantedById { get; set; }
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        public int AccessLevel { get; set; } = 2;
    }
}

namespace Main.Application.Dtos.Inventories.Index
{
    public class TagWithCountDto
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public int InventoryCount { get; set; }
        public int TotalUsageCount { get; set; }
    }
}

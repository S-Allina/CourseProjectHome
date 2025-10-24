namespace Main.Application.Dtos
{
    public class TagWithCountDto
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public int InventoryCount { get; set; }
        public int TotalUsageCount { get; set; }
    }
}

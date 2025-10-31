using Main.Application.Dtos.Inventories.Index;

namespace Main.Presentation.MVC.ViewModel
{
    public class HomeViewModel
    {
        public List<InventoryTableDto> UserInventories { get; set; } = new();
        public List<InventoryTableDto> SharedInventories { get; set; } = new();
        public List<InventoryTableDto> RecentInventories { get; set; } = new();
        public List<InventoryTableDto> PopularInventories { get; set; } = new();
        public string ActiveTab { get; set; } = "my-inventories";
    }
}

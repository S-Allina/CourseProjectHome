using Main.Application.Dtos.Inventories.Index;
using Main.Application.Dtos.Items.Create;
using Main.Application.Dtos.Items.Index;

namespace Main.Presentation.MVC.ViewModel
{
    public class ItemFormViewModel
    {
        public bool IsEditMode { get; set; }
        public InventoryDto Inventory { get; set; }
        public ItemDto Item { get; set; }
        public CreateItemDto CreateItem { get; set; }
        public List<ItemFieldValueDto> FieldValues { get; set; } = new();
    }
}

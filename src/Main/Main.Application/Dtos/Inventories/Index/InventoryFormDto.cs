using Main.Application.Dtos.Inventories.Create;
using Microsoft.AspNetCore.Http;

namespace Main.Application.Dtos.Inventories.Index
{
    public class InventoryFormDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsPublic { get; set; }
        public string CustomIdFormat { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Version { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<InventoryAccessDto> AccessList { get; set; } = new();
        public List<CreateInventoryFieldDto> Fields { get; set; } = new();

        public bool IsEditMode => Id.HasValue;
        public string FormAction => IsEditMode ? "Edit" : "Create";
        public string PageTitle => IsEditMode ? "Edit Inventory" : "Create New Inventory";
        public string SubmitButtonText => IsEditMode ? "Update Inventory" : "Create Inventory";
    }
}

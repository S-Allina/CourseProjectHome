using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos
{
    // Универсальный DTO для создания и редактирования
    public record InventoryFormDto
    {
        public int? Id { get; init; } // Nullable для создания
        public string Name { get; init; }
        public string Description { get; init; }
        public int? CategoryId { get; init; }
        public string ImageUrl { get; init; }
        public bool IsPublic { get; init; }
        public string CustomIdFormat { get; init; }
        public byte[] Version { get; set; }
        public List<string> Tags { get; init; } = new();
        public List<CreateInventoryFieldDto> Fields { get; init; } = new();

        // Дополнительные свойства для режима редактирования
        public bool IsEditMode => Id.HasValue;
        public string FormAction => IsEditMode ? "Edit" : "Create";
        public string PageTitle => IsEditMode ? "Edit Inventory" : "Create New Inventory";
        public string SubmitButtonText => IsEditMode ? "Update Inventory" : "Create Inventory";
    }
}

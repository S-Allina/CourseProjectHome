using Main.Application.Dtos.Inventories.Create;
using Main.Application.Dtos.Inventories.Index;
using Main.Application.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Main.Presentation.MVC.ViewModel
{
    public class InventoryFormViewModel : InventoryFormDto
    {            
            public List<SelectListItem> Categories { get; set; } = new();
            public bool IsEditMode => Id.HasValue;
        public string DescriptionHtml => MarkdownHelper.ConvertToHtml(Description);

        public string FormAction => IsEditMode ? "Edit" : "Create";
            public string PageTitle => IsEditMode ? "Edit Inventory" : "Create New Inventory";
            public string SubmitButtonText => IsEditMode ? "Update Inventory" : "Create Inventory";
        }
    }

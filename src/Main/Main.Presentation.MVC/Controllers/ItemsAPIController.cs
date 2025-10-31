using Main.Application.Dtos.Inventories.Index;
using Main.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Main.Presentation.MVC.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemsAPIController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IInventoryService _inventoryService;
        private readonly ICustomIdService _customIdService;

        public ItemsAPIController(IItemService itemService, IInventoryService inventoryService, ICustomIdService customIdService)
        {
            _itemService = itemService;
            _inventoryService = inventoryService;
            _customIdService = customIdService;
        }
    }
}

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


        [HttpGet("{inventoryId}/stats")]
        public async Task<ActionResult<InventoryStatsDto>> GetInventoryStats(int inventoryId)
        {
            try
            {
                var stats = await _itemService.GetInventoryStatsAsync(inventoryId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error calculating statistics" });
            }
        }

        [HttpGet("inventory/{inventoryId}/numeric")]
        public async Task<ActionResult<List<NumericFieldStats>>> GetNumericStats(int inventoryId)
        {
            try
            {
                var stats = await _itemService.GetNumericFieldStatsAsync(inventoryId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error calculating numeric statistics" });
            }
        }
    }
}

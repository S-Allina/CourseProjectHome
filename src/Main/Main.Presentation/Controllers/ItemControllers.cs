using Main.Application.Dtos;
using Main.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ILogger<InventoriesController> _logger;

        public ItemsController(
            IItemService itemService,
            ILogger<InventoriesController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ItemDto>> CreateItem(
            [FromBody] CreateItemDto createDto,
            CancellationToken cancellationToken = default)
        {
            var inventory = await _itemService.CreateAsync(createDto, cancellationToken);

            _logger.LogInformation("User created inventory {InventoryId}", inventory.Id);

            return CreatedAtAction("", new { id = inventory.Id }, inventory);
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IEnumerable<ItemDto>> GetItem(CancellationToken cancellationToken = default)
        //{
        //    return await _itemService.(cancellationToken);
        //}

        [HttpPost]
        public async Task<IActionResult> Delete(List<int> selectedIds) // Используйте тип вашего ID (int, Guid)
        {

            return RedirectToAction(nameof(Index));
        }

        //[HttpGet("{id}")]
        //[AllowAnonymous]
        //public async Task<InventoryDto> GetInventoryById(int id, CancellationToken cancellationToken = default)
        //{
        //    // Реализация получения инвентаря...
        //    return await _inventoryService.GetById(id, cancellationToken);
        //}

        //[HttpDelete("{id}")]
        //[AllowAnonymous]
        //public async Task<bool> DeleteInventory(int id, CancellationToken cancellationToken = default)
        //{
        //    return await _inventoryService.DeleteInventoryAsync(id, cancellationToken);
        //}

        //[HttpPut]
        //[AllowAnonymous]
        //public async Task<InventoryDto> UpdateInventory(InventoryDto inventoryDto, CancellationToken cancellationToken = default)
        //{
        //    return await _inventoryService.UpdateInventoryAsync(inventoryDto, cancellationToken);
        //}

        //private string GetCurrentUserId()
        //{
        //    return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //}
    }
}

using Main.Application.Dtos.Inventories.Index;
using Main.Application.Interfaces;
using Main.Domain.InterfacesRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main.Presentation.MVC.Controllers.API
{
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IInventoryFieldRepository _inventoryFieldRepository;
        private readonly ILogger<InventoriesController> _logger;

        public InventoryController(
            IInventoryService inventoryService, IInventoryFieldRepository inventoryFieldRepository,
            ILogger<InventoriesController> logger)
        {
            _inventoryFieldRepository = inventoryFieldRepository;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet("inventory")]
        public async Task<IEnumerable<InventoryTableDto>> GetInventory(CancellationToken cancellationToken = default)
        {
            return await _inventoryService.GetAll(cancellationToken);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<bool> Delete(int[] selectedIds, CancellationToken cancellationToken = default)
        {
            return await _inventoryService.DeleteInventoryAsync(selectedIds, cancellationToken);
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<InventoryDetailsDto> UpdateInventory(InventoryDetailsDto inventoryDto, CancellationToken cancellationToken = default)
        {
            return await _inventoryService.UpdateInventoryAsync(inventoryDto, cancellationToken);
        }
    }
}

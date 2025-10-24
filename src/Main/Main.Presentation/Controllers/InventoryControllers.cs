using Main.Application.Dtos;
using Main.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Main.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoriesController> _logger;

        public InventoriesController(
            IInventoryService inventoryService,
            ILogger<InventoriesController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateInventory(
            CreateInventoryDto createDto,
            CancellationToken cancellationToken = default)
        {
            var inventory = await _inventoryService.CreateInventoryAsync(createDto, cancellationToken);

            _logger.LogInformation("User created inventory {InventoryId}", inventory.Id);

            return CreatedAtAction(nameof(GetInventory), new { id = inventory.Id }, inventory);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<InventoryDto>> GetInventory(CancellationToken cancellationToken = default)
        {
            // Реализация получения инвентаря...
            return await _inventoryService.GetAll(cancellationToken);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<InventoryDto> GetInventoryById(int id, CancellationToken cancellationToken = default)
        {
            // Реализация получения инвентаря...
            return await _inventoryService.GetById(id, cancellationToken);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<bool> DeleteInventory(int[] id, CancellationToken cancellationToken = default)
        {
            return await _inventoryService.DeleteInventoryAsync(id, cancellationToken);
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<InventoryDto> UpdateInventory(InventoryDto inventoryDto, CancellationToken cancellationToken = default)
        {
            return await _inventoryService.UpdateInventoryAsync(inventoryDto, cancellationToken);
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}

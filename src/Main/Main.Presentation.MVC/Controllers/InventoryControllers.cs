using Main.Application.Dtos.Inventories.Index;
using Main.Application.Interfaces;
using Main.Domain.InterfacesRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Main.Presentation.MVC.Controllers
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

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<ActionResult<InventoryDto>> CreateInventory(
        //    [FromBody] CreateInventoryDto createDto,
        //    CancellationToken cancellationToken = default)
        //{
        //        var ownerId = "kjilsfhrfbeibkv ";
        //        if (string.IsNullOrEmpty(ownerId))
        //            return Unauthorized("User not authenticated");

        //        var inventory = await _inventoryService.CreateInventoryAsync(createDto, ownerId, cancellationToken);

        //        _logger.LogInformation("User {UserId} created inventory {InventoryId}", ownerId, inventory.Id);

        ////        return CreatedAtAction(nameof(GetInventory), new { id = inventory.Id }, inventory);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateInventory1(CreateInventoryDto createDto, CancellationToken cancellationToken = default)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Убедимся, что поля имеют правильные OrderIndex
        //            for (int i = 0; i < createDto.Fields.Count; i++)
        //            {
        //                createDto.Fields[i].OrderIndex = i + 1;
        //            }

        //            var ownerId = "current-user-id"; 
        //            var inventory = await _inventoryService.CreateInventoryAsync(createDto, ownerId, cancellationToken);

        //            _logger.LogInformation("User {UserId} created inventory {InventoryId} with {FieldCount} fields",
        //                ownerId, inventory.Id, createDto.Fields.Count);

        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", "Error creating inventory: " + ex.Message);
        //            _logger.LogError(ex, "Error creating inventory");
        //        }
        //    }

        //    // Если есть ошибки, возвращаем обратно с данными
        //    return Ok(createDto);
        //}

        [HttpGet("inventory")]
        [Authorize]
        public async Task<IEnumerable<InventoryDto>> GetInventory(CancellationToken cancellationToken = default)
        {
            // Реализация получения инвентаря...
            return await _inventoryService.GetAll(cancellationToken);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<bool> Delete(int[] selectedIds, CancellationToken cancellationToken = default)
        {
            return await _inventoryService.DeleteInventoryAsync(selectedIds, cancellationToken);
        }

        [HttpPut("update")]
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

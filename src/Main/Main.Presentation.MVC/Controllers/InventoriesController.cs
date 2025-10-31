using Main.Application.Dtos.Inventories.Create;
using Main.Application.Dtos.Inventories.Index;
using Main.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Main.Presentation.MVC.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IUsersService _usersService;
        private readonly ILogger<InventoriesController> _logger;
        private readonly IInventoryStatsService _statsService;
        public InventoriesController(IInventoryService inventoryService,
            ILogger<InventoriesController> logger,
            IInventoryStatsService statsService, IUsersService usersService)
        {
            _inventoryService = inventoryService;
            _logger = logger;
            _statsService = statsService;
            _usersService = usersService;
        }

        [HttpGet("Index")]
        [Authorize]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var t = await _inventoryService.GetAll(cancellationToken);
            return View(t);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateInventoryDto createDto, CancellationToken cancellationToken = default)
        {
            if (createDto.Fields != null)
            {
                foreach (var field in createDto.Fields)
                {
                    Console.WriteLine($"Field: {field.Name}, Type: {field.FieldType}");
                }
            }
            var inventory = await _inventoryService.CreateInventoryAsync(createDto, cancellationToken);

            //return CreatedAtAction(nameof(GetInventory), new { id = inventory.Id }, inventory);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int[] selectedIds)
        {
            await _inventoryService.DeleteInventoryAsync(selectedIds);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create()
        {
            var model = new InventoryFormDto();

            var categories = await _inventoryService.GetCategories(CancellationToken.None);

            var categoriesList = categories
        .OrderBy(c => c.Name)
        .Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();
            categoriesList.Add(new SelectListItem
            {
                Value = null,
                Text = "Другое"
            });
            ViewData["Categories"] = categoriesList;
            return View("Create", model);
        }
        public async Task<IActionResult> StatsPartial(int inventoryId)
        {
                var stats = await _statsService.GetInventoryStatsAsync(inventoryId);
                return PartialView("_StatisticsTab", stats);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var inventory = await _inventoryService.GetById(id);
            if (inventory == null) return NotFound();
            if (inventory.OwnerId == _usersService.GetCurrentUserId() || _usersService.GetCurrentUserRole() == "Admin")
            {
                var model = new InventoryFormDto
                {
                    Id = inventory.Id,
                    Name = inventory.Name,
                    Description = inventory.Description,
                    CategoryId = inventory.CategoryId,
                    ImageUrl = inventory.ImageUrl,
                    IsPublic = inventory.IsPublic,
                    OwnerId = inventory.OwnerId,
                    CustomIdFormat = inventory.CustomIdFormat,
                    Tags = inventory.Tags,
                    Version = Convert.ToBase64String(inventory.Version),
                    AccessList = inventory.AccessList,
                    Fields = inventory.Fields.Select(f => new CreateInventoryFieldDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Description = f.Description,
                        FieldType = f.FieldType,
                        OrderIndex = f.OrderIndex,
                        IsVisibleInTable = f.IsVisibleInTable,
                        IsRequired = f.IsRequired
                    }).ToList()
                };
                var categories = await _inventoryService.GetCategories(CancellationToken.None);
                var categoriesList = categories
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
                categoriesList.Add(new SelectListItem
                {
                    Value = null,
                    Text = "Другое"
                });
                ViewData["Categories"] = categoriesList;
                return View("Create", model);
            }
            else throw new UnauthorizedAccessException("У вас нет права редактирования инвенторя.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InventoryDetailsDto inventory, CancellationToken cancellationToken)
        {
            await _inventoryService.UpdateInventoryAsync(inventory);

            return RedirectToAction("Index", "Items", new { inventoryId = inventory.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AutoSave([FromBody] InventoryFormDto autoSaveDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var ownerId = "kjilsfhrfbeibkv";

                var existingInventory = await _inventoryService.GetById((int)autoSaveDto.Id, cancellationToken);
                if (existingInventory == null)
                {
                    return Json(new { success = false, message = "Inventory not found" });
                }
                var existingVersionString = Convert.ToBase64String(existingInventory.Version);
                if (existingVersionString != autoSaveDto.Version)
                {
                    return Json(new
                    {
                        success = false,
                        isConcurrencyError = true,
                        message = "This inventory was modified by another user. Please reload the page."
                    });
                }

                // Обновляем инвентарь
                var updateDto = new InventoryDetailsDto
                {
                    Id = (int)autoSaveDto.Id,
                    Name = autoSaveDto.Name,
                    Description = autoSaveDto.Description,
                    CategoryId = autoSaveDto.CategoryId,
                    ImageUrl = autoSaveDto.ImageUrl,
                    IsPublic = autoSaveDto.IsPublic,
                    CustomIdFormat = autoSaveDto.CustomIdFormat,
                    Tags = autoSaveDto.Tags != null ?
                        (autoSaveDto.Tags) :
                        new List<string>(),
                    Fields = autoSaveDto.Fields?.Select(f => new InventoryFieldDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                        FieldType = f.FieldType,
                        OrderIndex = f.OrderIndex,
                        Description = f.Description,
                        IsVisibleInTable = f.IsVisibleInTable,
                        IsRequired = f.IsRequired
                    }).ToList(),
                    Version = Convert.FromBase64String(autoSaveDto.Version),
                    OwnerId = ownerId
                };

                var updatedInventory = await _inventoryService.UpdateInventoryAsync(updateDto, cancellationToken);

                _logger.LogInformation("Inventory {InventoryId} auto-saved by user {UserId}", autoSaveDto.Id, ownerId);

                return Json(new
                {
                    success = true,
                    Version = updatedInventory.Version,
                    savedAt = DateTime.UtcNow
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new
                {
                    success = false,
                    isConcurrencyError = true,
                    message = "This inventory was modified by another user. Please reload the page."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Auto-save failed for inventory {InventoryId}", autoSaveDto?.Id);
                return Json(new { success = false, message = "Auto-save failed. Please try again." });
            }
        }
    }
}

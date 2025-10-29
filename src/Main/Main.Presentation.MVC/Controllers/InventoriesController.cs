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

        //// GET: Inventories
        [HttpGet("Index")]
        [Authorize]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var t = await _inventoryService.GetAll(cancellationToken);
            return View(t);
        }

        public class GetUsersDetailsRequest
        {
            public List<string> UserIds { get; set; } = new();
        }


            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]
            CreateInventoryDto createDto,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Fields count: {createDto.Fields?.Count}");
            if (createDto.Fields != null)
            {
                foreach (var field in createDto.Fields)
                {
                    Console.WriteLine($"Field: {field.Name}, Type: {field.FieldType}");
                }
            }
            var inventory = await _inventoryService.CreateInventoryAsync(createDto, cancellationToken);

            _logger.LogInformation("User created inventory {InventoryId}", inventory.Id);

            //return CreatedAtAction(nameof(GetInventory), new { id = inventory.Id }, inventory);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int[] selectedIds) // Используйте тип вашего ID (int, Guid)
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
            return View("Create", model); // Используем одно представление
        }
        public async Task<IActionResult> StatsPartial(int inventoryId)
        {
            try
            {
                var stats = await _statsService.GetInventoryStatsAsync(inventoryId);
                return PartialView("_StatisticsTab", stats);
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return PartialView("_StatsError", new { Message = "Ошибка загрузки статистики" });
            }
        }
        // GET: Edit
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
                return View("Create", model); // Используем то же представление
            }
            else throw new UnauthorizedAccessException("У вас нет права редактирования инвенторя.");
        }


        //public async Task<IActionResult> Items(int id, CancellationToken cancellationToken)
        //{
        //    return RedirectToAction("Index","items",id);
        //}
        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InventoryDto inventory, CancellationToken cancellationToken)
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

                // Проверяем, существует ли инвентарь и принадлежит ли пользователю
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
                var updateDto = new InventoryDto
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
        //// GET: Inventories/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var inventory = await _context.Inventories
        //        .Include(i => i.Category)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (inventory == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(inventory);
        //}

        //// GET: Inventories/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var inventory = await _context.Inventories
        //        .Include(i => i.Category)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (inventory == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(inventory);
        //}

        //// POST: Inventories/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var inventory = await _context.Inventories.FindAsync(id);
        //    if (inventory != null)
        //    {
        //        _context.Inventories.Remove(inventory);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

    }
}

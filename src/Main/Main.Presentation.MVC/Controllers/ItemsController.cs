using Main.Application.Dtos.Items.Create;
using Main.Application.Dtos.Items.Index;
using Main.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main.Presentation.MVC.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IInventoryService _inventoryService;
        private readonly ICustomIdService _customIdService;

        public ItemsController(IItemService itemService, IInventoryService inventoryService, ICustomIdService customIdService)
        {
            _itemService = itemService;
            _inventoryService = inventoryService;
            _customIdService = customIdService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? inventoryId, CancellationToken cancellationToken)
        {
            var items = await _itemService.GetByInventoryAsync(inventoryId.Value, cancellationToken);
            var inventory = await _inventoryService.GetById(inventoryId.Value, cancellationToken);

            ViewBag.SelectedInventory = inventory;
            return View(items.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create(int inventoryId, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryService.GetById(inventoryId, cancellationToken);
            if (inventory == null)
            {
                return NotFound();
            }

            var createDto = new CreateItemDto
            {
                InventoryId = inventoryId,
                FieldValues = inventory.Fields.Select(f => new CreateItemFieldValueDto
                {
                    InventoryFieldId = f.Id,
                    FieldName = f.Name,
                    FieldType = f.FieldType,
                    IsRequired = f.IsRequired
                }).ToList()
            };

            ViewBag.Inventory = inventory;
            ViewData["CustomId"] = await _customIdService.GenerateCustomIdAsync(inventoryId, cancellationToken);
            return View(createDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateItemDto createDto, CancellationToken cancellationToken)
        {
            var item = await _itemService.CreateAsync(createDto, cancellationToken);
            return RedirectToAction("Index", "Items", new { inventoryId = createDto.InventoryId });

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int itemId, CancellationToken cancellationToken)
        {
            var item = await _itemService.GetByIdAsync(itemId, cancellationToken);
            var inventory = await _inventoryService.GetById(item.InventoryId, cancellationToken);
            if (inventory == null)
            {
                return NotFound();
            }

            var createDto = new CreateItemDto
            {
                Id = item.Id,
                InventoryId = item.InventoryId,
                CustomId = item.CustomId,
                CreatedById = item.CreatedById,
                CreatedAt = item.CreatedAt,
                FieldValues = inventory.Fields.Select(f => new CreateItemFieldValueDto
                {
                    InventoryFieldId = f.Id,
                    FieldName = f.Name,
                    FieldType = f.FieldType,
                    TextValue = item.FieldValues.First(i => i.InventoryFieldId == f.Id).TextValue,
                    MultilineTextValue = item.FieldValues.First(i => i.InventoryFieldId == f.Id).MultilineTextValue,
                    BooleanValue = item.FieldValues.First(i => i.InventoryFieldId == f.Id).BooleanValue,
                    FileUrl = item.FieldValues.First(i => i.InventoryFieldId == f.Id).FileUrl,
                    NumberValue = item.FieldValues.First(i => i.InventoryFieldId == f.Id).NumberValue,
                    IsRequired = f.IsRequired
                }).ToList()
            };

            ViewBag.Inventory = inventory;
            ViewData["CustomId"] = await _customIdService.GenerateCustomIdAsync(item.InventoryId, cancellationToken);
            return View(createDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(ItemDto dto, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryService.GetById(dto.InventoryId, cancellationToken);

            await _itemService.UpdateItemAsync(dto, cancellationToken);

            TempData["SuccessMessage"] = "Item updated successfully!";
            return RedirectToAction("Index", new { inventoryId = dto.InventoryId });

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int[] selectedIds)
        {
            var id = await _itemService.DeleteItemAsync(selectedIds);
            return RedirectToAction("Index", "Items", new { inventoryId = id });
        }
    }
}

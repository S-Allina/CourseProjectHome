using Main.Application.Dtos;
using Main.Application.Interfaces;
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

        // GET: Items
        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int? inventoryId, CancellationToken cancellationToken)
        {
            // Если передан inventoryId, показываем товары этого инвентаря
            if (inventoryId.HasValue)
            {
                var items = await _itemService.GetByInventoryAsync(inventoryId.Value, cancellationToken);
                var inventory = await _inventoryService.GetById(inventoryId.Value, cancellationToken);

                ViewBag.SelectedInventory = inventory;
                return View(items);
            }

            // Иначе показываем список инвентарей
            var inventories = await _inventoryService.GetAll(cancellationToken);
            return View(inventories);
        }

        // Метод для получения товаров в формате JSON (для AJAX)
        [HttpGet]
        public async Task<IActionResult> GetInventoryItems(int inventoryId, CancellationToken cancellationToken)
        {
            var items = await _itemService.GetByInventoryAsync(inventoryId, cancellationToken);
            var inventory = await _inventoryService.GetById(inventoryId, cancellationToken);

            return Json(new { Items = items, Inventory = inventory });
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            ////var item = await _context.Items
            ////    .Include(i => i.Inventory)
            ////    .FirstOrDefaultAsync(m => m.Id == id);
            //if (item == null)
            //{
            //    return NotFound();
            //}

            return View();
        }
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

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateItemDto createDto, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _itemService.CreateAsync(createDto, cancellationToken);
                return RedirectToAction("Index", "Items", new { inventoryId = createDto.InventoryId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating item: " + ex.Message);
            }
            var inventory = await _inventoryService.GetById(createDto.InventoryId, cancellationToken);

            ViewBag.Inventory = inventory;
            ViewData["CustomId"] = await _customIdService.GenerateCustomIdAsync(createDto.InventoryId, cancellationToken);

            return View(createDto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var item = await _itemService.GetByIdAsync(id, cancellationToken);
            try
            {

                if (item == null)
                {
                    return NotFound();
                }

                var inventory = await _inventoryService.GetById(item.InventoryId, cancellationToken);

                ViewBag.Inventory = inventory;
                return View(item);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { inventoryId = item?.InventoryId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ItemDto dto, CancellationToken cancellationToken)
        {
            dto = dto with { CreatedById = "dfhbmcnv" };

            var inventory = await _inventoryService.GetById(dto.InventoryId, cancellationToken);



            await _itemService.UpdateItemAsync(dto, cancellationToken);

            TempData["SuccessMessage"] = "Item updated successfully!";
            return RedirectToAction("Index", new { inventoryId = dto.InventoryId });

        }
        //}

        //// POST: Items/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int[] selectedIds)
        {
            var id = await _itemService.DeleteItemAsync(selectedIds);
            return RedirectToAction("Index", "Items", new { inventoryId = id });
        }

        //private bool ItemExists(int id)
        //{
        //    return _context.Items.Any(e => e.Id == id);
        //}
    }
}

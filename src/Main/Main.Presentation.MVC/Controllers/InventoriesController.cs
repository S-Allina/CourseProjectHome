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
        private readonly IInventoryStatsService _statsService;
        private readonly ILogger<InventoriesController> _logger;

        public InventoriesController(
            IInventoryService inventoryService,
            IInventoryStatsService statsService,
            ILogger<InventoriesController> logger)
        {
            _inventoryService = inventoryService;
            _statsService = statsService;
            _logger = logger;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var inventories = await _inventoryService.GetAll(cancellationToken);
            return View(inventories);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var model = await _inventoryService.GetCreateViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateInventoryDto createDto, CancellationToken cancellationToken = default)
        {
            var inventory = await _inventoryService.CreateInventoryAsync(createDto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _inventoryService.GetEditViewModelAsync(id);
            if (model == null) return NotFound();

            return View("Create", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InventoryDetailsDto inventory, CancellationToken cancellationToken)
        {
            await _inventoryService.UpdateInventoryAsync(inventory, cancellationToken);
            return RedirectToAction("Index", "Items", new { inventoryId = inventory.Id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int[] selectedIds)
        {
            await _inventoryService.DeleteInventoryAsync(selectedIds);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> StatsPartial(int inventoryId)
        {
            var stats = await _statsService.GetInventoryStatsAsync(inventoryId);
            return PartialView("~/Views/Inventories/Partials/_StatisticsTab.cshtml", stats);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AutoSave([FromBody] InventoryFormDto autoSaveDto, CancellationToken cancellationToken = default)
        //{
        //    var result = await _inventoryService.AutoSaveAsync(autoSaveDto);

        //    return Json(result);
        //}
    }
}

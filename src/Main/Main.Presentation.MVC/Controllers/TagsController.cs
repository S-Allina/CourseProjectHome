using Main.Application.Dtos;
using Main.Application.Interfaces;
using Main.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Presentation.MVC.Controllers
{
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;
        private readonly IInventoryService _inventoryService;

        public TagsController(ITagService tagService, IInventoryService inventoryService)
        {
            _tagService = tagService;
            _inventoryService = inventoryService;
        }

        // Облако тегов
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tags = await _tagService.GetAllTagsWithCountAsync();
            return View(tags);
        }

        // Поиск инвентарей по тегу
        [HttpGet]
        public async Task<IActionResult> SearchByTag(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                return RedirectToAction(nameof(Index));
            }

            var inventories = await _inventoryService.GetInventoriesByTagAsync(tagName);

            // Используем существующую модель поиска
            var searchResult = new GlobalSearchResult
            {
                SearchTerm = tagName,
                Inventories = inventories
            };

            return View("~/Views/Search/Index.cshtml", searchResult);
        }
    }
}

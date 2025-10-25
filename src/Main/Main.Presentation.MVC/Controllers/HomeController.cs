using Main.Application.Dtos.Inventories.Index;
using Main.Application.Interfaces;
using Main.Presentation.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Main.Presentation.MVC.Controllers
{

    public class HomeController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var userInventoriesTask = await _inventoryService.GetUserInventoriesAsync(userId, cancellationToken);
                var sharedInventoriesTask = await _inventoryService.GetSharedInventoriesAsync(userId, cancellationToken);

                var model = new HomeViewModel
                {
                    UserInventories = userInventoriesTask.ToList(),
                    SharedInventories = sharedInventoriesTask.ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page for user {UserId}", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View(new HomeViewModel());
            }
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
    public class HomeViewModel
    {
        public List<InventoryDto> UserInventories { get; set; } = new();
        public List<InventoryDto> SharedInventories { get; set; } = new();
        public string ActiveTab { get; set; } = "my-inventories";
    }
}

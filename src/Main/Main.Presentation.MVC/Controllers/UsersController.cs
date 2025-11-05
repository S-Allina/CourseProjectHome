using Main.Application.Interfaces;
using Main.Presentation.MVC.Controllers.API;
using Main.Presentation.MVC.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Main.Presentation.MVC.Controllers
{
    [Route("Users")]
    public class UsersController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<UsersAPIController> _logger;

        public UsersController(IInventoryService inventoryService, ILogger<UsersAPIController> logger, IUsersService usersService)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userInventoriesTask = await _inventoryService.GetUserInventoriesAsync(cancellationToken);
            var sharedInventoriesTask = await _inventoryService.GetSharedInventoriesAsync(cancellationToken);

            var model = new HomeViewModel
            {
                UserInventories = userInventoriesTask.ToList(),
                SharedInventories = sharedInventoriesTask.ToList()
            };

            return View(model);
        }
    }
}

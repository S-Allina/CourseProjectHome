using Main.Application.Interfaces;
using Main.Domain.entities.common;
using Main.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Main.Presentation.MVC.Controllers
{
    [Route("Users")]
    public class UsersController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IUsersService _usersService;
        private readonly ILogger<UsersAPIController> _logger;

        public UsersController(IInventoryService inventoryService, ILogger<UsersAPIController> logger, IUsersService usersService)
        {
            _inventoryService = inventoryService;
            _logger = logger;
            _usersService = usersService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page for user {UserId}", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View(new HomeViewModel());
            }
        }

    }
}

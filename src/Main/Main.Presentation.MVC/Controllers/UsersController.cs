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

        
    }
}

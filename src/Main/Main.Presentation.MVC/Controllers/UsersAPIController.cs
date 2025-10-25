using Main.Domain.entities.common;
using Main.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Main.Presentation.MVC.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersAPIController> _logger;

        public UsersAPIController(ApplicationDbContext context, ILogger<UsersAPIController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            try
            {
                _logger.LogInformation("Creating user in Main database: {Email}", request.Email);

                // Проверяем, существует ли уже пользователь
                var existingUser = await _context.Users.FindAsync(request.Id);
                if (existingUser != null)
                {
                    _logger.LogInformation("User already exists: {UserId}", request.Id);
                    return Ok(new { message = "User already exists" });
                }

                // Создаем нового пользователя
                var user = new User
                {
                    Id = request.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User created successfully: {UserId}", request.Id);
                return Ok(new { message = "User created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Email}", request.Email);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                _logger.LogInformation("Updating user in Main database: {UserId}", id);

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User updated successfully: {UserId}", id);
                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
        [HttpPost("UpdateTheme")]
        public async Task<IActionResult> UpdateTheme([FromBody] ThemeModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Обновляем claim пользователя
                var identity = (ClaimsIdentity)User.Identity;
                var themeClaim = identity.FindFirst("Theme");

                if (themeClaim != null)
                    identity.RemoveClaim(themeClaim);

                var t = model.Theme.ToString();

                identity.AddClaim(new Claim("Theme", "dark"));
            }

            return Ok();
        }
    }
    public class ThemeModel
    {
        public string Theme { get; set; }
    }

    public class CreateUserRequest
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

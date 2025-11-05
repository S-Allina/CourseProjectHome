using Identity.Application.Dto;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Identity.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public string? GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
            return userId;
        }

        public async Task<UserDto> GetCurrentUserAsync()
        {
            var id = GetUserId();
            return await _userService.GetByIdAsync(id);
        }
    }
}

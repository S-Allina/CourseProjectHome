using Identity.Application.Dto;

namespace Identity.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? GetUserId();
        Task<UserDto> GetCurrentUserAsync();
    }
}

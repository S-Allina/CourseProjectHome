using Main.Application.Dtos.Common;
using Main.Domain.entities.common;

namespace Main.Application.Interfaces
{
    public interface IUsersService
    {
        string? GetCurrentUserId();
        string? GetCurrentUserRole();
        Task<UserDto> GetCurrentUser();
        bool CheckBlock(string[] ids);
        Task<string> CreateUser(CreateUserRequest request);
        Task<bool> DeleteUsersAsync(string[] ids);
    }
}

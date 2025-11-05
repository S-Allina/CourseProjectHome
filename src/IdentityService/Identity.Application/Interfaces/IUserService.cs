using Identity.Application.Dto;
using Identity.Domain.Entity;
using Identity.Domain.Enums;
using Users.Application.Dto;

namespace Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(string id, CancellationToken cancellationToken=default);
        Task<ResponseDto> GetAllAsync(CancellationToken cancellationToken);
        Task<ResponseDto> UpdateUsersStatusAsync(IEnumerable<string> userIds, Func<ApplicationUser, Statuses> statusSelector, CancellationToken cancellationToken);
        Task<ResponseDto> UpdateUsersRoleAsync(IEnumerable<string> userIds, Roles role, CancellationToken cancellationToken);
        Task<ResponseDto> BlockUser(IEnumerable<string> userIds, CancellationToken cancellationToken);
        Task<ResponseDto> UnlockUser(IEnumerable<string> userIds, CancellationToken cancellationToken);
        Task<ResponseDto> DeleteSomeUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken);
        Task<ResponseDto> DeleteUnconfirmedUsersAsync(CancellationToken cancellationToken);
    }
}

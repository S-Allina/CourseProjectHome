using Identity.Application.Dto;
using Identity.Domain.Entity;
using Identity.Domain.Enums;

namespace Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<ResponseDto> GetAllAsync(CancellationToken cancellationToken);
        Task<ResponseDto> UpdateUsersStatusAsync(IEnumerable<string> userIds, Func<ApplicationUser, Statuses> statusSelector, CancellationToken cancellationToken);
        Task<ResponseDto> UpdateUsersRoleAsync(IEnumerable<string> userIds, Roles role, CancellationToken cancellationToken);
        Task<ResponseDto> CheckBlockAsync(string id, CancellationToken cancellationToken);
        Task<ResponseDto> StatusChangeAsync(IEnumerable<string> userIds, Statuses role, CancellationToken cancellationToken);
        Task<ResponseDto> DeleteSomeUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken);
        Task<ResponseDto> DeleteUnconfirmedUsersAsync(CancellationToken cancellationToken);
    }
}

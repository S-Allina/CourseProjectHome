using Identity.Application.Dto;
using Identity.Domain.Entity;

namespace Identity.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> LoginAsync(UserLoginRequestDto request);
        Task ForgotPasswordAsync();
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}

using Identity.Application.Dto;
using Identity.Domain.Entity;

namespace Identity.Application.Interfaces
{
    public interface IAuthService
    {
        Task<CurrentUserDto> LoginAsync(UserLoginRequestDto request);
        Task ForgotPasswordAsync();
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}

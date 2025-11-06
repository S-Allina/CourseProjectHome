using Identity.Application.Dto;

namespace Identity.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> LoginAsync(UserLoginRequestDto request);
        Task ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}

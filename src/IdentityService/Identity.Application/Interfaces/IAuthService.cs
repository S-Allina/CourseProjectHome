using Identity.Application.Dto;
using Identity.Domain.Entity;

namespace Identity.Application.Interfaces
{
    public interface IAuthService
    {
        Task<CurrentUserDto> LoginAsync(UserLoginRequestDto request);
        Task<CurrentUserDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<RevokeTokenResponseDto> RevokeRefreshTokenAsync(RevokeTokenRequestDto request);
        Task ForgotPasswordAsync();
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        //Task<CurrentUserDto> GoogleCallBackAsync(string returnUrl, string remoteError);
    }
}

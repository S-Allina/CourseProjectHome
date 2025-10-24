using Identity.Domain.Entity;

namespace Identity.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    }
}

using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Identity.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Identity.Presentation.Extention
{
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subjectId);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim("sub", user.Id),
            new Claim("name", user.UserName!),
            new Claim("email", user.Email ?? ""),
            new Claim("Status", user.Status.ToString())
        };

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim("role", role));
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                if (context.RequestedClaimTypes != null && context.RequestedClaimTypes.Any())
                {
                    claims = claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)).ToList();
                }

                context.IssuedClaims = claims;
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subjectId);
            context.IsActive = user != null;
        }
    }
}
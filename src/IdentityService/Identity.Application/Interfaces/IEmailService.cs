using Identity.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> ConfirmEmailAsync(string token, string email);
        Task SendVerificationEmailAsync(ApplicationUser user);
        Task SendPasswordResetEmailAsync(ApplicationUser user, string token);
    }
}

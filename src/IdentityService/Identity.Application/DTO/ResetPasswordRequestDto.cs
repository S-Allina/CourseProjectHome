using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Dto
{
    public record ResetPasswordDto
    {
        public string Token { get; init; }
        public string Email { get; init; }
        public string NewPassword { get; init; }
        public object ConfirmPassword { get; init; }
    }
}

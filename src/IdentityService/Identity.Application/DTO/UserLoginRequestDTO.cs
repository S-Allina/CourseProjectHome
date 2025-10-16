using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Dto
{
    public record UserLoginRequestDto
    {
        public string Email { get; init;}
        public string Password { get; init;}
    }
}

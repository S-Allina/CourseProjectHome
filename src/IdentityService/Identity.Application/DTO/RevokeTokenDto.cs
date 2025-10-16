using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entity
{
    public record RevokeTokenRequestDto
    {
        public string Token { get; init; }
    }

    public class RevokeTokenResponseDto
    {
        public string Message { get; init; }
    }

    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; init; }
    }
}

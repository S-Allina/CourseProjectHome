using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entity
{
    public record RevokeTokenRequest
    {
        public string Token { get; init; }
    }

    public class RevokeTokenResponse
    {
        public string Message { get; init; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; init; }
    }
}

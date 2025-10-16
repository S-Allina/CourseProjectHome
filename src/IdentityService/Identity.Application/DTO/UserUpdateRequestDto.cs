using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Dto
{
    public record UserUpdateRequestDto
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Gender { get; init; }
        public string PhoneNumber { get; init; }
    }
}

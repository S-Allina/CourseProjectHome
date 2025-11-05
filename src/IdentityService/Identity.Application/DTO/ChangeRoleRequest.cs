using Identity.Domain.Enums;

namespace Identity.Application.DTO
{
    public class ChangeRoleRequest
    {
        public required string[] UserIds { get; set; }
        public Roles Role { get; set; }
    }
}

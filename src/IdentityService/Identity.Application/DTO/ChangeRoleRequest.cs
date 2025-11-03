using Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTO
{
    public class ChangeRoleRequest
    {
        public string[] UserIds { get; set; }
        public Roles Role { get; set; }
    }
}

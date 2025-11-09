using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Interfaces.Identity
{
    public interface IIdentityApiClient
    {
        Task<bool> CheckBlockAsync(string userId);
    }
}

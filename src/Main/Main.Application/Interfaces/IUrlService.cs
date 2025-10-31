using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Interfaces
{
    public interface IUrlService
    {
        string GetAuthFrontUrl();
        string BuildFrontendUrl(string path);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Configuration
{
    public class UrlSettings
    {
        public string Main { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
        public string AuthFront { get; set; } = string.Empty;
    }
}

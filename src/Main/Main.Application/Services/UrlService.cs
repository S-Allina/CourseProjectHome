using Main.Application.Configuration;
using Main.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Services
{
    public class UrlService : IUrlService
    {
        private readonly UrlSettings _urlSettings;

        public UrlService(IOptions<UrlSettings> urlSettings)
        {
            _urlSettings = urlSettings.Value;
        }

        public string GetAuthFrontUrl() => _urlSettings.AuthFront;

        public string BuildFrontendUrl(string path)
        {
            return $"{_urlSettings.AuthFront}/theapp/#/theapp{path}";
        }
    }
}

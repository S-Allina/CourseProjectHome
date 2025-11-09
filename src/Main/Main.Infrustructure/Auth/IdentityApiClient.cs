using Main.Application.Dto;
using Main.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Main.Infrastructure.Auth
{
    public class IdentityApiClient 
    {
        private readonly HttpClient _httpClient;
        private readonly UrlSettings _urlSettings;

        public IdentityApiClient(HttpClient httpClient, IOptions<UrlSettings> urlSettings)
        {
            _httpClient = httpClient;
            _urlSettings = urlSettings.Value;

            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "AuthService");
        }

        public async Task<bool> CheckBlockAsync(string userId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_urlSettings.Auth}/api/user", userId);

            if (response.IsSuccessStatusCode)
            {
                return (bool)response?.Content?.ReadFromJsonAsync<ResponseDto>()?.Result?.Result == false;
            }
            else
            {
                return false;
            }
        }
    }
}

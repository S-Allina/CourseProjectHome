using Identity.Application.Configuration;
using Identity.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Services
{
    public class MainApiClient : IMainApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MainApiClient> _logger;
        private readonly UrlSettings _urlSettings;

        public MainApiClient(HttpClient httpClient, ILogger<MainApiClient> logger, IOptions<UrlSettings> urlSettings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _urlSettings = urlSettings.Value;

            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "AuthService");
        }

        public async Task<bool> CreateUserAsync(string userId, string firstName, string lastName, string email)
        {
            var request = new
            {
                Id = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };

            var response = await _httpClient.PostAsJsonAsync($"{_urlSettings.Main}/api/users", request);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("User successfully created in Main API: {UserId}", userId);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to create user in Main API. Status: {StatusCode}, UserId: {UserId}",
                    response.StatusCode, userId);
                return false;
            }
        }

        public async Task<bool> NotifyBlockedUsers(string[] blockedUserIds)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_urlSettings.Main}/api/users/blocked-users",
                blockedUserIds
            );

            return response.IsSuccessStatusCode;
        }
    }
}

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

            // Настройка HttpClient
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "AuthService");
        }

        public async Task<bool> CreateUserAsync(string userId, string firstName, string lastName, string email)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Main API to create user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(string userId, string firstName, string lastName, string email)
        {
            try
            {
                var request = new 
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                };

                var response = await _httpClient.PutAsJsonAsync($"{_urlSettings.Main}/api/users/{userId}", request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User successfully updated in Main API: {UserId}", userId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to update user in Main API. Status: {StatusCode}, UserId: {UserId}",
                        response.StatusCode, userId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Main API to update user: {UserId}", userId);
                return false;
            }
        }
    }
}

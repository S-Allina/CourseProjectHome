using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Main.Presentation.MVC.Controllers
{
    [Route("auth/{**catchAll}")]
    public class AuthProxyController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthProxyController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AuthService");
        }

        [HttpGet]
        public async Task<IActionResult> ProxyAuth()
        {
            var path = Request.Path.ToString().Replace("/auth/", "/");
            var query = Request.QueryString.ToString();

            var response = await _httpClient.GetAsync($"{path}{query}");

            if (response.Content.Headers.ContentType?.MediaType?.Contains("text/html") == true)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "text/html");
            }

            return new ContentResult
            {
                Content = await response.Content.ReadAsStringAsync(),
                ContentType = response.Content.Headers.ContentType?.ToString(),
                StatusCode = (int)response.StatusCode
            };
        }

        [HttpPost]
        public async Task<IActionResult> ProxyAuthPost()
        {
            var path = Request.Path.ToString().Replace("/auth/", "/");

            using var streamReader = new StreamReader(Request.Body);
            var body = await streamReader.ReadToEndAsync();

            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(path, content);

            return new ContentResult
            {
                Content = await response.Content.ReadAsStringAsync(),
                ContentType = response.Content.Headers.ContentType?.ToString(),
                StatusCode = (int)response.StatusCode
            };
        }
    }
}
using Main.Application.Dtos.Common;
using Main.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace Main.Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AlertService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Success(string message) => AddAlert("success", "Success", message);
        public void Error(string message) => AddAlert("danger", "Error", message);
        public void Warning(string message) => AddAlert("warning", "Warning", message);

        private void AddAlert(string type, string title, string message)
        {
            var tempData = _httpContextAccessor.HttpContext?.Items["TempData"] as ITempDataDictionary;
            if (tempData != null)
                tempData["Alert"] = JsonSerializer.Serialize(new AlertModelDto { Type = type, Title = title, Message = message });

        }

        public AlertModelDto? GetAlert()
        {
            var tempData = _httpContextAccessor.HttpContext?.Items["TempData"] as ITempDataDictionary;
            var alertJson = tempData?["Alert"] as string;
            return alertJson != null ? JsonSerializer.Deserialize<AlertModelDto>(alertJson) : null;
        }
    }
}

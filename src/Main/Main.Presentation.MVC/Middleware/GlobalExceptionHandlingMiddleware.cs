using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace Main.Presentation.MVC.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(context, ex);
            }
        }

        private void HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string message;
            string alertType = "danger";

            switch (exception)
            {
                case FluentValidation.ValidationException fluentValidationException:
                    message = "Ошибка валидации: " + fluentValidationException.Message;
                    alertType = "warning";
                    _logger.LogWarning(exception, "FluentValidation error: {Message}", message);
                    break;

                case ArgumentException argumentException:
                    message = argumentException.Message;
                    alertType = "warning";
                    _logger.LogWarning(exception, "ArgumentException: {Message}", message);
                    break;

                case KeyNotFoundException keyNotFoundException:
                    message = "Ресурс не найден: " + keyNotFoundException.Message;
                    alertType = "warning";
                    _logger.LogWarning(exception, "KeyNotFoundException: {Message}", message);
                    break;

                case UnauthorizedAccessException unauthorizedAccessException:
                    message = "Доступ запрещен: " + exception.Message;
                    alertType = "danger";
                    _logger.LogWarning(exception, "UnauthorizedAccessException: {Message}", message);
                    break;

                case DbUpdateConcurrencyException dbUpdateConcurrencyException:
                    message = "Конфликт данных: " + dbUpdateConcurrencyException.Message;
                    alertType = "warning";
                    _logger.LogWarning(exception, "DbUpdateConcurrencyException: {Message}", message);
                    break;

                case InvalidOperationException invalidOperationException:
                    message = "Ошибка операции: " + invalidOperationException.Message;
                    alertType = "warning";
                    _logger.LogWarning(exception, "InvalidOperationException: {Message}", message);
                    break;

                default:
                    message = "Произошла непредвиденная ошибка. Пожалуйста, попробуйте позже.";
                    alertType = "danger";
                    _logger.LogCritical(exception, "Unhandled exception: {Message}", exception.Message);
                    break;
            }

            HandleException(context, message, alertType);
        }

        private void HandleException(HttpContext context, string message, string alertType)
        {
            var tempDataFactory = context.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = tempDataFactory.GetTempData(context);

            tempData["ErrorAlertMessage"] = message;
            tempData["ErrorAlertType"] = alertType;
            tempData.Save();

            var referer = context.Request.Headers["Referer"].ToString();
            var redirectUrl = !string.IsNullOrEmpty(referer) ? referer : "/";
            context.Response.Redirect(redirectUrl);
        }
    }
}
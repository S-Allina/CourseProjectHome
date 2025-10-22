using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;
            List<string?> errors = null;

            switch (exception)
            {
                //case Product.Domain.Exceptions.ProductException validationException:

                //    status = HttpStatusCode.BadRequest;
                //    _response.DisplayMessage = "Ошибка валидации.";
                //    _response.ErrorMessages = validationException.Errors;
                //    _response.IsSuccess = false;

                //    _logger.LogWarning(exception, "Product validation error: {Message}", _response.ErrorMessages);
                //    break;

                case FluentValidation.ValidationException fluentValidationException:
                    message = "Ошибка валидации." + fluentValidationException.Message;
                    errors = fluentValidationException.Errors.Select(e => e.ErrorMessage).ToList();

                    _logger.LogWarning(exception, "FluentValidation error: {Message}", errors);
                    break;

                case ArgumentException argumentException:
                    message = argumentException.Message;

                    _logger.LogWarning(exception, "ArgumentException: {Message}", message);
                    break;

                case KeyNotFoundException keyNotFoundException:
                    message = keyNotFoundException.Message;

                    _logger.LogWarning(exception, "KeyNotFoundException: {Message}", message);
                    break;

                case UnauthorizedAccessException unauthorizedAccessException:
                    message = exception.Message;

                    _logger.LogWarning(exception, "UnauthorizedAccessException: {Message}", message);
                    break;

                case DbUpdateConcurrencyException dbUpdateConcurrencyException:
                    message = dbUpdateConcurrencyException.Message;

                    _logger.LogWarning(exception, "DbUpdateConcurrencyException, {Message}", message);
                    break;
                case InvalidOperationException invalidOperationException:
                    message = invalidOperationException.Message;

                    _logger.LogWarning(exception, "DbUpdateConcurrencyException, {Message}", message);
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "Произошла непредвиденная ошибка." + exception.Message;

                    _logger.LogCritical(exception, "Unhandled exception: {Message}", exception.Message);
                    break;
            }


            // Для AJAX запросов
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                var result = JsonSerializer.Serialize(new { error = message });
                await context.Response.WriteAsync(result);
                return;
            }

            // Для обычных запросов сохраняем ошибку в TempData и редиректим на страницу ошибки
            var tempDataProvider = context.RequestServices.GetService<ITempDataProvider>();
            if (tempDataProvider != null)
            {
                var tempData = tempDataProvider.LoadTempData(context);
                tempData["Error"] = message;
                tempDataProvider.SaveTempData(context, tempData);
            }

            // Редирект на красивую страницу ошибки
            context.Response.Redirect("/Error");
        }
    }
}

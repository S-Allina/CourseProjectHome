using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace Identity.Presentation.Middleware
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
                case ArgumentException argumentException:
                    status = HttpStatusCode.BadRequest;
                    message = argumentException.Message;

                    _logger.LogWarning(exception, "ArgumentException: {Message}", message);
                    break;

                case KeyNotFoundException keyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = keyNotFoundException.Message;

                    _logger.LogWarning(exception, "KeyNotFoundException: {Message}", message);
                    break;

                case UnauthorizedAccessException unauthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    message = unauthorizedAccessException.Message;

                    _logger.LogWarning(exception, "UnauthorizedAccessException: {Message}", message);
                    break;

                case DbUpdateConcurrencyException dbUpdateConcurrencyException:
                    status = HttpStatusCode.Unauthorized;
                    message = dbUpdateConcurrencyException.Message;

                    _logger.LogWarning(exception, "DbUpdateConcurrencyException: {Message}", message);
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "Произошла непредвиденная ошибка." + exception.Message;

                    _logger.LogCritical(exception, "Unhandled exception: {Message}", exception.Message);
                    break;
            }

            _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var errorResponse = new
            {
                message,
                errors
            };

            var json = JsonSerializer.Serialize(errorResponse);

            await context.Response.WriteAsync(json);
        }
    }
}

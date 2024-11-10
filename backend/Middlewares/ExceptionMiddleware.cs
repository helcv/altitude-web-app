using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace backend.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (InvalidJwtException ex)
            {
                _logger.LogError(ex, "Invalid JWT token provided.");
                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, "Invalid Token", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Server error", "An internal server error");
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string type, string detail)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            ProblemDetails problem = new()
            {
                Status = (int)statusCode,
                Type = type,
                Detail = detail
            };

            var json = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(json);
        }
    }
}

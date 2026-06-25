using System.Net;
using System.Text.Json;
using RoadReady.API.Models;

namespace RoadReady.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex,"Unhandled Exception: {Message}",ex.Message);

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response already started. Cannot handle exception globally.");

                    return;
                }

                await HandleExceptionAsync(context,ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context,Exception exception)
        {
            HttpStatusCode statusCode =HttpStatusCode.InternalServerError;

            string message ="An unexpected error occurred.";

            switch (exception)
            {
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;

                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Forbidden;
                    message = exception.Message;
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "Internal Server Error";
                    break;
            }

            var response =
                new ErrorResponse
                {
                    StatusCode = (int)statusCode,
                    Message = message,
                    Details = exception.Message
                };

            context.Response.Clear();

            context.Response.ContentType ="application/json";

            context.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy =
                        JsonNamingPolicy.CamelCase
                };

            return context.Response.WriteAsync(JsonSerializer.Serialize(
                    response,
                    options));
        }
    }
}
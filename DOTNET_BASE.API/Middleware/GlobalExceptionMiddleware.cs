using System.Net;
using System.Text.Json;

namespace DOTNET_BASE.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
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
        var requestId = context.Items["RequestId"]?.ToString() ?? Guid.NewGuid().ToString();

        _logger.LogError(exception,
            "An unhandled exception occurred - RequestId: {RequestId} - Path: {Path} - Method: {Method}",
            requestId, context.Request.Path, context.Request.Method);

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            RequestId = requestId,
            Message = GetErrorMessage(exception),
            StatusCode = GetStatusCode(exception),
            Timestamp = DateTime.UtcNow
        };

        if (_environment.IsDevelopment())
        {
            errorResponse.Details = exception.ToString();
        }

        response.StatusCode = errorResponse.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            NotImplementedException => (int)HttpStatusCode.NotImplemented,
            TimeoutException => (int)HttpStatusCode.RequestTimeout,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => "Required parameter is missing",
            ArgumentException => "Invalid request parameters",
            KeyNotFoundException => "Resource not found",
            UnauthorizedAccessException => "Unauthorized access",
            NotImplementedException => "Feature not implemented",
            TimeoutException => "Request timeout",
            _ => "An internal server error occurred"
        };
    }

    public class ErrorResponse
    {
        public string RequestId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Details { get; set; }
    }
}
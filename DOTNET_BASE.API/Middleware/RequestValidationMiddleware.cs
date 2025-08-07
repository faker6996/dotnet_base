using System.Text;
using System.Text.Json;

namespace DOTNET_BASE.API.Middleware;

public class RequestValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestValidationMiddleware> _logger;
    private readonly RequestValidationOptions _options;

    public RequestValidationMiddleware(RequestDelegate next, ILogger<RequestValidationMiddleware> logger, RequestValidationOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!await ValidateRequestAsync(context))
        {
            return;
        }

        await _next(context);
    }

    private async Task<bool> ValidateRequestAsync(HttpContext context)
    {
        var request = context.Request;

        if (!ValidateContentType(context))
        {
            await WriteErrorResponse(context, 400, "Invalid content type");
            return false;
        }

        if (!ValidateContentLength(context))
        {
            await WriteErrorResponse(context, 413, "Request entity too large");
            return false;
        }

        if (!await ValidateJsonBodyAsync(context))
        {
            await WriteErrorResponse(context, 400, "Invalid JSON format");
            return false;
        }

        if (!ValidateHeaders(context))
        {
            await WriteErrorResponse(context, 400, "Required headers missing");
            return false;
        }

        return true;
    }

    private static bool ValidateContentType(HttpContext context)
    {
        var request = context.Request;

        if (request.Method == "GET" || request.Method == "DELETE")
            return true;

        if (request.ContentType != null && 
            request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            return true;

        if (request.ContentLength == 0)
            return true;

        return false;
    }

    private bool ValidateContentLength(HttpContext context)
    {
        return context.Request.ContentLength <= _options.MaxContentLengthBytes;
    }

    private async Task<bool> ValidateJsonBodyAsync(HttpContext context)
    {
        var request = context.Request;

        if (request.ContentLength == 0 || !request.Body.CanRead)
            return true;

        if (request.ContentType == null || 
            !request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            return true;

        try
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            
            var totalBytesRead = 0;
            var bytesToRead = buffer.Length;
            
            while (totalBytesRead < bytesToRead)
            {
                var bytesRead = await request.Body.ReadAsync(buffer.AsMemory(totalBytesRead, bytesToRead - totalBytesRead));
                if (bytesRead == 0)
                    break;
                totalBytesRead += bytesRead;
            }
            
            request.Body.Position = 0;

            var json = Encoding.UTF8.GetString(buffer, 0, totalBytesRead);
            
            if (string.IsNullOrWhiteSpace(json))
                return true;

            JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning("Invalid JSON in request body: {Error}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating JSON body");
            return false;
        }
    }

    private static bool ValidateHeaders(HttpContext context)
    {
        var request = context.Request;

        if (request.Method == "POST" || request.Method == "PUT")
        {
            if (request.ContentLength > 0 && 
                !request.Headers.ContainsKey("Content-Type"))
            {
                return false;
            }
        }

        if (request.Headers.ContainsKey("User-Agent"))
        {
            var userAgent = request.Headers["User-Agent"].ToString();
            if (string.IsNullOrWhiteSpace(userAgent) || userAgent.Length > 500)
            {
                return false;
            }
        }

        return true;
    }

    private async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        var requestId = context.Items["RequestId"]?.ToString() ?? Guid.NewGuid().ToString();
        
        _logger.LogWarning("Request validation failed - RequestId: {RequestId} - {Message}", 
            requestId, message);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            requestId,
            message,
            statusCode,
            timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
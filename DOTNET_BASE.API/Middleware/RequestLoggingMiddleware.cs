using System.Diagnostics;
using System.Text;

namespace DOTNET_BASE.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();
        
        context.Items["RequestId"] = requestId;

        var request = context.Request;
        var requestBody = await GetRequestBodyAsync(request);

        _logger.LogInformation(
            "Request Started - {RequestId} {Method} {Path} {QueryString} - Body: {Body}",
            requestId,
            request.Method,
            request.Path,
            request.QueryString,
            requestBody);

        var originalResponseBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            var response = context.Response;
            var responseBodyContent = await GetResponseBodyAsync(responseBody);
            
            await responseBody.CopyToAsync(originalResponseBodyStream);
            context.Response.Body = originalResponseBodyStream;

            _logger.LogInformation(
                "Request Completed - {RequestId} {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms - Response: {Response}",
                requestId,
                request.Method,
                request.Path,
                response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                responseBodyContent);
        }
    }

    private static async Task<string> GetRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength == 0 || !request.Body.CanRead)
            return string.Empty;

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
        
        return Encoding.UTF8.GetString(buffer, 0, totalBytesRead);
    }

    private static async Task<string> GetResponseBodyAsync(MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var response = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);
        
        return response;
    }
}
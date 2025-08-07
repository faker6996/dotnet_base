using System.Collections.Concurrent;
using System.Net;

namespace DOTNET_BASE.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;
    private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = new();

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, RateLimitOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var now = DateTime.UtcNow;

        var clientInfo = _clients.AddOrUpdate(clientId, 
            new ClientRequestInfo { LastRequestTime = now, RequestCount = 1 },
            (key, existingInfo) => UpdateClientInfo(existingInfo, now));

        if (clientInfo.RequestCount > _options.MaxRequestsPerWindow)
        {
            _logger.LogWarning("Rate limit exceeded for client {ClientId}. Requests: {RequestCount}", 
                clientId, clientInfo.RequestCount);

            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers["X-RateLimit-Limit"] = _options.MaxRequestsPerWindow.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = "0";
            context.Response.Headers["X-RateLimit-Reset"] = GetResetTime(clientInfo.WindowStart).ToString();
            
            await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            return;
        }

        context.Response.Headers["X-RateLimit-Limit"] = _options.MaxRequestsPerWindow.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = 
            Math.Max(0, _options.MaxRequestsPerWindow - clientInfo.RequestCount).ToString();
        context.Response.Headers["X-RateLimit-Reset"] = GetResetTime(clientInfo.WindowStart).ToString();

        await _next(context);

        CleanupExpiredClients();
    }

    private string GetClientIdentifier(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private ClientRequestInfo UpdateClientInfo(ClientRequestInfo existingInfo, DateTime now)
    {
        var timeSinceWindowStart = now - existingInfo.WindowStart;

        if (timeSinceWindowStart >= _options.WindowSize)
        {
            return new ClientRequestInfo
            {
                LastRequestTime = now,
                RequestCount = 1,
                WindowStart = now
            };
        }

        existingInfo.LastRequestTime = now;
        existingInfo.RequestCount++;
        return existingInfo;
    }

    private static long GetResetTime(DateTime windowStart)
    {
        return ((DateTimeOffset)windowStart.AddMinutes(1)).ToUnixTimeSeconds();
    }

    private void CleanupExpiredClients()
    {
        var cutoffTime = DateTime.UtcNow - _options.WindowSize - TimeSpan.FromMinutes(1);
        var expiredClients = _clients.Where(kvp => kvp.Value.LastRequestTime < cutoffTime)
                                   .Select(kvp => kvp.Key)
                                   .ToList();

        foreach (var clientId in expiredClients)
        {
            _clients.TryRemove(clientId, out _);
        }
    }

    private class ClientRequestInfo
    {
        public DateTime LastRequestTime { get; set; }
        public int RequestCount { get; set; }
        public DateTime WindowStart { get; set; } = DateTime.UtcNow;
    }
}

public class RateLimitOptions
{
    public int MaxRequestsPerWindow { get; set; } = 100;
    public TimeSpan WindowSize { get; set; } = TimeSpan.FromMinutes(1);
}
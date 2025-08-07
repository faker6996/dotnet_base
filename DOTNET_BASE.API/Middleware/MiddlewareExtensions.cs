namespace DOTNET_BASE.API.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }

    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }

    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder, RateLimitOptions? options = null)
    {
        options ??= new RateLimitOptions();
        return builder.UseMiddleware<RateLimitingMiddleware>(options);
    }

    public static IApplicationBuilder UseRequestValidation(this IApplicationBuilder builder, RequestValidationOptions? options = null)
    {
        options ??= new RequestValidationOptions();
        return builder.UseMiddleware<RequestValidationMiddleware>(options);
    }

    public static IServiceCollection AddCustomMiddleware(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RateLimitOptions>(options =>
        {
            options.MaxRequestsPerWindow = configuration.GetValue<int>("RateLimit:MaxRequestsPerWindow", 100);
            options.WindowSize = TimeSpan.FromMinutes(configuration.GetValue<int>("RateLimit:WindowSizeMinutes", 1));
        });

        services.Configure<RequestValidationOptions>(options =>
        {
            options.MaxContentLengthMB = configuration.GetValue<int>("RequestValidation:MaxContentLengthMB", 10);
            options.MaxContentLengthBytes = configuration.GetValue<long>("RequestValidation:MaxContentLengthBytes", 10 * 1024 * 1024);
            options.ValidateJsonFormat = configuration.GetValue<bool>("RequestValidation:ValidateJsonFormat", true);
            options.ValidateContentType = configuration.GetValue<bool>("RequestValidation:ValidateContentType", true);
            options.ValidateHeaders = configuration.GetValue<bool>("RequestValidation:ValidateHeaders", true);
        });

        return services;
    }

    public static IApplicationBuilder UseCustomMiddlewarePipeline(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseGlobalExceptionHandler();
        
        app.UseRequestLogging();
        
        var validationOptions = new RequestValidationOptions();
        configuration.GetSection("RequestValidation").Bind(validationOptions);
        app.UseRequestValidation(validationOptions);
        
        var rateLimitOptions = new RateLimitOptions();
        configuration.GetSection("RateLimit").Bind(rateLimitOptions);
        app.UseRateLimiting(rateLimitOptions);

        return app;
    }
}
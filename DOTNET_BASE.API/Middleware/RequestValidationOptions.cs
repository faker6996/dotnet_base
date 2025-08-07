namespace DOTNET_BASE.API.Middleware;

public class RequestValidationOptions
{
    public int MaxContentLengthMB { get; set; } = 10;
    public long MaxContentLengthBytes { get; set; } = 10 * 1024 * 1024;
    public bool ValidateJsonFormat { get; set; } = true;
    public bool ValidateContentType { get; set; } = true;
    public bool ValidateHeaders { get; set; } = true;
}
# Middleware Implementation Demo

## Đã triển khai thành công các middleware sau:

### 1. **RequestLoggingMiddleware**
- **Chức năng**: Ghi log toàn bộ request/response
- **Lợi ích**: Theo dõi API usage, debug issues, performance monitoring
- **Features**:
  - Unique RequestId cho mỗi request
  - Log request body và response body
  - Đo thời gian xử lý (Duration)
  - Structured logging format

### 2. **GlobalExceptionMiddleware** 
- **Chức năng**: Xử lý exception toàn hệ thống
- **Lợi ích**: Consistent error responses, không expose internal errors
- **Features**:
  - Convert exceptions thành HTTP status codes phù hợp
  - Hide sensitive error details ở Production
  - Show full stack trace ở Development
  - Structured error response format

### 3. **RateLimitingMiddleware**
- **Chức năng**: Giới hạn số request per IP
- **Lợi ích**: Bảo vệ API khỏi spam, DDoS attacks
- **Features**:
  - Configurable limits (default: 100 requests/minute)
  - Per-IP tracking
  - Rate limit headers (X-RateLimit-*)
  - Automatic cleanup expired entries

### 4. **RequestValidationMiddleware**
- **Chức năng**: Validate request format và headers
- **Lợi ích**: Early validation, security, consistent input format
- **Features**:
  - Content-Type validation
  - Configurable Content-Length limits
  - JSON format validation
  - Required headers validation
  - Environment-specific settings

## Configuration

### appsettings.json (Production)
```json
{
  "RateLimit": {
    "MaxRequestsPerWindow": 100,
    "WindowSizeMinutes": 1
  },
  "RequestValidation": {
    "MaxContentLengthMB": 10,
    "MaxContentLengthBytes": 10485760,
    "ValidateJsonFormat": true,
    "ValidateContentType": true,
    "ValidateHeaders": true
  }
}
```

### appsettings.Development.json
```json
{
  "RequestValidation": {
    "MaxContentLengthMB": 50,
    "MaxContentLengthBytes": 52428800
  }
}
```

### Environment-specific Settings:
- **Production**: 10 MB max request size (security-focused)
- **Development**: 50 MB max request size (testing-friendly)
- All validation options configurable per environment

## Middleware Pipeline Order

```csharp
// Thứ tự middleware trong pipeline
app.UseGlobalExceptionHandler();    // 1. Bắt exceptions đầu tiên
app.UseRequestLogging();           // 2. Log requests
app.UseRequestValidation();        // 3. Validate input
app.UseRateLimiting();            // 4. Check rate limits
// ... built-in middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
```

## Testing

### 1. Test Request Logging
```bash
curl -X GET "https://localhost:7094/api/users" -H "Content-Type: application/json"
```
**Result**: Check logs để thấy request/response được ghi lại

### 2. Test Exception Handling
```bash
curl -X GET "https://localhost:7094/api/users/99999"
```
**Result**: Trả về structured error response thay vì exception

### 3. Test Rate Limiting
```bash
# Gửi > 100 requests trong 1 phút
for i in {1..105}; do curl -X GET "https://localhost:7094/api/users"; done
```
**Result**: Request thứ 101+ sẽ return 429 Too Many Requests

### 4. Test Request Validation
```bash
curl -X POST "https://localhost:7094/api/users" \
  -H "Content-Type: application/json" \
  -d "invalid json{"
```
**Result**: Trả về 400 Bad Request với message "Invalid JSON format"

## Benefits Achieved

✅ **Cross-cutting Concerns**: Tách biệt logging, validation khỏi business logic
✅ **Security**: Rate limiting, input validation, safe error handling  
✅ **Observability**: Comprehensive logging với RequestId tracking
✅ **Maintainability**: Centralized middleware configuration
✅ **Performance**: Early validation và caching mechanisms
✅ **Consistency**: Uniform error responses và request handling

## Run Project
```bash
cd DOTNET_BASE.API
dotnet run
```

API sẽ chạy tại: https://localhost:7094 với đầy đủ middleware protection!
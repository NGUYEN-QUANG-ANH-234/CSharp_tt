namespace DemoWebAPI.WebAPI.Middlewares
{
    // Middlewares/CorrelationIdMiddleware.cs
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeader = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Lấy hoặc tạo mới Correlation ID
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            // 2. Thêm vào Response Header để client có thể check
            context.Response.Headers.Append(CorrelationIdHeader, correlationId);

            // 3. Đẩy vào Serilog LogContext để mọi dòng log trong request này đều có ID này
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}

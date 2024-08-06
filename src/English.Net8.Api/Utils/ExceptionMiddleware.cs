namespace English.Net8.Api.Utils
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

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
            catch (HttpRequestException ex)
            {
                await HandleExceptionAsync(context, ex, (int)ex.StatusCode!);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex, int statusCode = 500)
        {
            _logger.LogError(ex.Message);

            context.Response.StatusCode = statusCode;
            return Task.CompletedTask;
        }
    }
}

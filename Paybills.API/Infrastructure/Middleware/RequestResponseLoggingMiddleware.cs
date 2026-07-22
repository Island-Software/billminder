using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Paybills.API.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                await _next(context);
            }
            finally
            {
                var requestMethod = SanitizeForLog(context.Request?.Method);
                var requestPath = SanitizeForLog(context.Request?.Path.Value);

                _logger.LogInformation(
                    "Request {method} {url} => {statusCode}",
                    requestMethod,
                    requestPath,
                    context.Response?.StatusCode);
            }
        }

        private static string SanitizeForLog(string value)
        {
            if (value == null)
            {
                return null;
            }

            return value
                .Replace("\r\n", " ", StringComparison.Ordinal)
                .Replace("\n", " ", StringComparison.Ordinal)
                .Replace("\r", " ", StringComparison.Ordinal)
                .Replace("\u2028", " ", StringComparison.Ordinal)
                .Replace("\u2029", " ", StringComparison.Ordinal);
        }
    }
}
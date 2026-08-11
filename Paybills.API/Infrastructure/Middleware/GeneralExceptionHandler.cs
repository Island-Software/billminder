using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paybills.API.Infrastructure.Errors;

namespace Paybills.API.Infrastructure.Middleware
{
    public class GeneralExceptionHandler : IExceptionHandler
    {
        private readonly IHostEnvironment _environment;
        private readonly ILogger<GeneralExceptionHandler> _logger;

        public GeneralExceptionHandler(ILogger<GeneralExceptionHandler> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _environment.IsDevelopment()
                ? new ApiException(httpContext.Response.StatusCode, exception.Message, exception.StackTrace?.ToString())
                : new ApiException(httpContext.Response.StatusCode, "Internal Server Error", exception.Message);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);

            await httpContext.Response.WriteAsync(json, cancellationToken);

            return true; // Handled
        }
    }
}
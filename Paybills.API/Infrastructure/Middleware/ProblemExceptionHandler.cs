using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Paybills.API.Middleware
{
    public class ProblemException : Exception
    {
        public string Error { get; }
        public string Message { get; }

        public ProblemException(string error, string message) : base(message)
        {
            Error = error;
            Message = message;
        }
    }

    public class ProblemExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        public ProblemExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not ProblemException problemException)
            {
                return false; // Let other handlers process this
            }

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = problemException.Error,
                Detail = problemException.Message,
                Type = "Bad Request"
            };

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetails
                });
        }
    } 
}
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SITRA.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var problemDetails = exception switch
            {
                BadHttpRequestException or JsonException => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = SITRA.Core.Helpers.Utilities.InvalidRequestBodyTitleMessage,
                    Detail = SITRA.Core.Helpers.Utilities.InvalidRequestBodyDetailMessage,
                    Instance = httpContext.Request.Path
                },

                UnauthorizedAccessException => new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = SITRA.Core.Helpers.Utilities.UnauthorizedTitleMessage,
                    Detail = SITRA.Core.Helpers.Utilities.UnauthorizedDetailMessage,
                    Instance = httpContext.Request.Path
                },

                ValidationException validationEx => new ProblemDetails
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = SITRA.Core.Helpers.Utilities.ValidationErrorTitleMessage,
                    Detail = string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage)),
                    Instance = httpContext.Request.Path
                },

                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = SITRA.Core.Helpers.Utilities.InternalServerErrorTitleMessage,
                    Detail = _env.IsDevelopment()
                        ? exception.Message
                        : SITRA.Core.Helpers.Utilities.InternalServerErrorDetailMessage,
                    Instance = httpContext.Request.Path
                }
            };

            _logger.LogError(exception, "Unhandled exception occurred. Status: {StatusCode}, Path: {Path}",
                problemDetails.Status, httpContext.Request.Path);

            httpContext.Response.StatusCode = problemDetails.Status!.Value;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

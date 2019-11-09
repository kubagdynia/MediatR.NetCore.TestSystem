using Kernel.Exceptions;
using Kernel.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Kernel.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            LogException(exception);

            ErrorResponse errorResponse = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode
            };

            if (exception is DomainException domainException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                foreach (var error in domainException.DomainErrors)
                {
                    errorResponse.AddError(
                        code: error.ErrorCode,
                        userMessage: error.ErrorMessage,
                        details: $"Validation for '{error.PropertyName}' with value '{error.AttemptedValue}' failed in {error.ClassName}",
                        message: "Validation failed");
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.AddError(
                        code: nameof(HttpStatusCode.InternalServerError),
                        details: exception.StackTrace,
                        message: exception.Message,
                        userMessage: string.Empty);
            }

            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(errorResponse.ToString());
        }

        private void LogException(Exception exception)
        {
            _logger.LogError($"Exception occured {exception}");
        }

    }
}

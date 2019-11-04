using Kernel.Exceptions;
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
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            string message = exception.Message;

            BaseResponse baseResponse = new BaseResponse
            {
                StatusCode = context.Response.StatusCode
            };

            if (exception is DomainException domainException)
            {
                foreach (var error in domainException.DomainErrors)
                {
                    baseResponse.Errors.Add(new Error
                    {
                        Code = error.ErrorCode,
                        UserMessage = error.ErrorMessage,
                        Details = error.PropertyName
                    });
                }
            }

            return context.Response.WriteAsync(baseResponse.ToString());
        }

    }
}

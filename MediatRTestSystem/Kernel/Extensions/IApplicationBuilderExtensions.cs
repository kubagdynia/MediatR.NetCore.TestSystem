using Kernel.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Kernel.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseErrorHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}

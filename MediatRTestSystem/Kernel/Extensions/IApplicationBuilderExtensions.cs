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

        public static void UseCustomSwagger(this IApplicationBuilder app, string url = "/swagger/v1/swagger.json", string name = "My API V1")
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(url: url, name: name);
            });
        }
    }
}

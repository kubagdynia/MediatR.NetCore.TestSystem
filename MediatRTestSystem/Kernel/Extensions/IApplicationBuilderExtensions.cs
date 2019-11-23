using Hangfire;
using Kernel.Configurations;
using Kernel.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

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

        public static void UseCustomHangfire(this IApplicationBuilder app, IConfiguration config, bool useDashboard = true)
        {
            HangfireConfiguration? hangfireConfiguration = config.GetSection(HangfireConfiguration.SectionName).Get<HangfireConfiguration>();

            if (hangfireConfiguration is null || !hangfireConfiguration.Enabled || !hangfireConfiguration.UseDashboard)
            {
                return;
            }

            app.UseHangfireDashboard();
        }
    }
}

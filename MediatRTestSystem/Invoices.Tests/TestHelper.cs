using FluentValidation;
using Invoices.Models;
using Invoices.Repositories;
using Kernel.Behaviors;
using Kernel.Configurations;
using Kernel.Messages;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Invoices.Tests
{
    public static class TestHelper
    {
        public static ServiceProvider PrepareServiceProvider()
        {
            ServiceCollection services = PrepareServiceCollection();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        public static ServiceCollection PrepareServiceCollection()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<IInvoiceRepository, InvoiceMemoryRepository>();

            services.AddValidatorsFromAssemblies(new[] { typeof(Invoice).Assembly });

            services.AddMediatR(typeof(Invoice));

            services.Configure<HangfireConfiguration>(configuration => configuration.Enabled = false);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            services.AddScoped<IMessageExecutor, MessageExecutor>();
            services.AddScoped<IMessageManager, MessageManager>();

            return services;
        }
    }
}

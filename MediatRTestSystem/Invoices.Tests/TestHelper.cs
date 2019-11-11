using FluentValidation;
using Invoices.Models;
using Invoices.Repositories;
using Kernel.Behaviors;
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

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            return services;
        }
    }
}

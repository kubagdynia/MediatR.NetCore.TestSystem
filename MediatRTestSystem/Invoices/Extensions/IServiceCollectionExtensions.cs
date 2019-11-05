using Invoices.Repositories;
using Kernel.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Invoices
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddInvoices(this IServiceCollection services, string optionsSectionName = "invoices", bool registerValidators = true)
        {
            services.AddKernel(assembly: Assembly.GetExecutingAssembly(), registerValidators: true);

            services.AddScoped<IInvoiceRepository, InvoiceMemoryRepository>();

            return services;
        }
    }
}

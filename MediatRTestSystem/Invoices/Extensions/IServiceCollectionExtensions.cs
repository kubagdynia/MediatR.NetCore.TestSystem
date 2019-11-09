using Invoices.Models;
using Invoices.Repositories;
using Kernel.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Invoices
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddInvoices(this IServiceCollection services, string optionsSectionName = "invoices", bool registerValidators = true)
        {
            services.AddKernel(assembly: Assembly.GetExecutingAssembly(), registerValidators: true);

            services.AddSingleton<IInvoiceRepository>(c => new InvoiceMemoryRepository(new List<Invoice>
            {
                new Invoice(id: Guid.NewGuid(), number: "J/1/2019", creationDate: new DateTime(2019, 10, 1)),
                new Invoice(id: Guid.NewGuid(), number: "J/2/2019", creationDate: new DateTime(2019, 10, 25)),
                new Invoice(id: Guid.NewGuid(), number: "J/3/2019", creationDate: new DateTime(2019, 11, 1))
            }));

            return services;
        }
    }
}

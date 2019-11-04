using Invoices.Models;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace Api.SwaggerExamples.Requests
{
    public class CreateInvoiceExample : IExamplesProvider<Invoice>
    {
        public Invoice GetExamples()
        {
            return new Invoice
            {
                Id = Guid.NewGuid(),
                Number = "JK/8/10/2019",
                CreationDate = DateTime.Now
            };
        }
    }
}

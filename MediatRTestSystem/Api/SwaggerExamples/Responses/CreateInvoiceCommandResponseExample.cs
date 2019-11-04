using Invoices.Commands;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace Api.SwaggerExamples.Responses
{
    public class CreateInvoiceCommandResponseExample : IExamplesProvider<CreateInvoiceCommandResponse>
    {
        public CreateInvoiceCommandResponse GetExamples()
        {
            return new CreateInvoiceCommandResponse
            {
                Id = Guid.NewGuid()
            };
        }
    }
}

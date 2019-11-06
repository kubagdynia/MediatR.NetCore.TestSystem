using Api.Contracts.V1.Responses;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace Api.SwaggerExamples.Responses
{
    public class CreateInvoiceResponseExample : IExamplesProvider<CreateInvoiceResponse>
    {
        public CreateInvoiceResponse GetExamples() => new CreateInvoiceResponse(id: Guid.NewGuid());
    }
}

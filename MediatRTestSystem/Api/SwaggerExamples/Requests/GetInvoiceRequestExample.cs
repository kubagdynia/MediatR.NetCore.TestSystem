using Api.Contracts.V1.Requests;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace Api.SwaggerExamples.Requests
{
    public class GetInvoiceRequestExample : IExamplesProvider<GetInvoiceRequest>
    {
        public GetInvoiceRequest GetExamples() => new GetInvoiceRequest { Id = Guid.NewGuid() };
    }
}

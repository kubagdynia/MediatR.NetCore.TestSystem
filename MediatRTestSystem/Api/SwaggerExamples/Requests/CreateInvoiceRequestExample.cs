using Api.Contracts.V1.Requests;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace Api.SwaggerExamples.Requests
{
    public class CreateInvoiceRequestExample : IExamplesProvider<CreateInvoiceRequest>
    {
        public CreateInvoiceRequest GetExamples() => new CreateInvoiceRequest(number: "JK/8/10/2019", DateTime.Now);
    }
}

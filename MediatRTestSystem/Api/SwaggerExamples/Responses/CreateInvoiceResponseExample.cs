using Api.Contracts.V1.Responses;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace Api.SwaggerExamples.Responses
{
    public class CreateInvoiceResponseExample : IExamplesProvider<CreateInvoiceResponse>
    {
        public CreateInvoiceResponse GetExamples() => new CreateInvoiceResponse(new IdDto(Guid.NewGuid()), statusCode: StatusCodes.Status201Created);
    }
}

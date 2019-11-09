using Api.Contracts.V1.Responses;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;

namespace Api.SwaggerExamples.Responses
{
    public class InvoicesResponseExample : IExamplesProvider<InvoicesResponse>
    {
        public InvoicesResponse GetExamples()
        {
            var invoices = new InvoicesResponse(
                new List<InvoiceDto>
                {
                    new InvoiceDto(id: Guid.NewGuid(), "JK/1/11/2019", creationDate: new DateTime(2019, 11, 5, 20, 34, 10)),
                    new InvoiceDto(id: Guid.NewGuid(), "JK/2/11/2019", creationDate: new DateTime(2019, 11, 20, 14, 12, 45)),
                }, StatusCodes.Status200OK);

            return invoices;
        }
    }
}

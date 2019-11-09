using Kernel.Responses.Api;
using System;

namespace Api.Contracts.V1.Responses
{
    public class CreateInvoiceResponse : Response<IdDto>
    {
        public CreateInvoiceResponse(IdDto result, int statusCode) : base(result, statusCode)
        {
        }

        public CreateInvoiceResponse(Guid id, int statusCode) : base(new IdDto(id), statusCode)
        {

        }
    }
}

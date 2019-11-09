using Kernel.Responses.Api;
using System.Collections.Generic;

namespace Api.Contracts.V1.Responses
{
    public class InvoicesResponse : Response<IEnumerable<InvoiceDto>>
    {
        public InvoicesResponse(IEnumerable<InvoiceDto> result, int statusCode) : base(result, statusCode)
        {
        }
    }
}

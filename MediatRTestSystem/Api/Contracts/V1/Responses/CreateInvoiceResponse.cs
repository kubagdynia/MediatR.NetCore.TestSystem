using System;

namespace Api.Contracts.V1.Responses
{
    public class CreateInvoiceResponse
    {
        public Guid Id { get; set; }

        public CreateInvoiceResponse(Guid id)
        {
            Id = id;
        }
    }
}

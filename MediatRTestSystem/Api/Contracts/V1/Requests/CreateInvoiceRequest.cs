using System;

namespace Api.Contracts.V1.Requests
{
    public class CreateInvoiceRequest
    {
        public string? Number { get; set; }
        public DateTime CreationDate { get; set; }

        public CreateInvoiceRequest()
        {

        }

        public CreateInvoiceRequest(string? number, DateTime creationDate)
        {
            Number = number;
            CreationDate = creationDate;
        }
    }
}

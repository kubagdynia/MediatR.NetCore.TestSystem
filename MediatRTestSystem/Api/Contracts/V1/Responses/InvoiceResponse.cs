using System;

namespace Api.Contracts.V1.Responses
{
    public class InvoiceResponse
    {
        public Guid Id { get; set; }
        public string? Number { get; set; }
        public DateTime CreationDate { get; set; }

        public InvoiceResponse(Guid id, string? number, DateTime creationDate)
        {
            Id = id;
            Number = number;
            CreationDate = creationDate;
        }
    }
}

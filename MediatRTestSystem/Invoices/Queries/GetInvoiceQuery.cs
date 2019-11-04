using MediatR;
using System;

namespace Invoices.Queries
{
    public class GetInvoiceQuery : IRequest<GetInvoiceQueryResponse>
    {
        public GetInvoiceQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}

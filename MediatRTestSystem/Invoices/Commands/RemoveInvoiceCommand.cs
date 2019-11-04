using MediatR;
using System;

namespace Invoices.Commands
{
    public class RemoveInvoiceCommand : IRequest<RemoveInvoiceCommandResponse>
    {
        public Guid Id { get; }

        public RemoveInvoiceCommand(Guid id)
        {
            Id = id;
        }
    }
}

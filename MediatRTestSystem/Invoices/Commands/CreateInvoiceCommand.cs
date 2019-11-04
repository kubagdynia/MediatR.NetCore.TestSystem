using Invoices.Models;
using MediatR;

namespace Invoices.Commands
{
    public class CreateInvoiceCommand : IRequest<CreateInvoiceCommandResponse>
    {
        public Invoice Invoice { get; }

        public CreateInvoiceCommand(Invoice invoice)
        {
            Invoice = invoice;
        }
    }
}

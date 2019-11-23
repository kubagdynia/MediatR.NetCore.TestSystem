using Invoices.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Invoices.Commands.Handlers
{
    public class RemoveInvoiceHandler : IRequestHandler<RemoveInvoiceCommand, RemoveInvoiceCommandResponse>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public RemoveInvoiceHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }
        public Task<RemoveInvoiceCommandResponse> Handle(RemoveInvoiceCommand request, CancellationToken cancellationToken)
        {
            RemoveInvoiceCommandResponse response = new RemoveInvoiceCommandResponse
            {
                Removed = _invoiceRepository.Remove(request.Id)
            };

            return Task.FromResult(response);
        }
    }
}

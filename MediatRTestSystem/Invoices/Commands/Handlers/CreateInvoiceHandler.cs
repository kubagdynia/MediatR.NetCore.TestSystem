using Invoices.Events;
using Invoices.Repositories;
using Kernel.Messages;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Invoices.Commands.Handlers
{
    public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, CreateInvoiceCommandResponse>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMessageManager _messageManager;

        public CreateInvoiceHandler(IInvoiceRepository invoiceRepository, IMessageManager messageManager)
        {
            _invoiceRepository = invoiceRepository;
            _messageManager = messageManager;
        }

        public Task<CreateInvoiceCommandResponse> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            CreateInvoiceCommandResponse response = new CreateInvoiceCommandResponse();

            //response.AddError("Error from command hadler");
            //if (response.Errors != null)
            //{
            //    return Task.FromResult(response);
            //}

            var id = _invoiceRepository.Create(request.Invoice);
            response.Id = id;

            _messageManager.Publish(new InvoiceCreatedEvent(id));

            return Task.FromResult(response);
        }
    }
}

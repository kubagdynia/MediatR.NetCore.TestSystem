using Invoices.Events;
using Invoices.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Invoices.Commands.Handlers
{
    public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, CreateInvoiceCommandResponse>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMediator _mediator;

        public CreateInvoiceHandler(IInvoiceRepository invoiceRepository, IMediator mediator)
        {
            _invoiceRepository = invoiceRepository;
            _mediator = mediator;
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

            _mediator.Publish(new InvoiceCreatedEvent(id));

            return Task.FromResult(response);
        }
    }
}

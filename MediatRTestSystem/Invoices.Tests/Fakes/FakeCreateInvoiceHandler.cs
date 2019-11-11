using Invoices.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Invoices.Tests.Fakes
{
    public class FakeCreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, CreateInvoiceCommandResponse>
    {
        private Counter _counter;

        public FakeCreateInvoiceHandler(Counter counter)
        {
            _counter = counter;
        }

        public Task<CreateInvoiceCommandResponse> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            _counter.Increment();

            CreateInvoiceCommandResponse result = new CreateInvoiceCommandResponse { Id = Guid.NewGuid() };

            return Task.FromResult(result);

        }
    }
}

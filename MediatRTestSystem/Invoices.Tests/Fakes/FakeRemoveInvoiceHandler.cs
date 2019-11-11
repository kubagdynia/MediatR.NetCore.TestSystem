using Invoices.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Invoices.Tests.Fakes
{
    public class FakeRemoveInvoiceHandler : IRequestHandler<RemoveInvoiceCommand, RemoveInvoiceCommandResponse>
    {
        private Counter _counter;

        public FakeRemoveInvoiceHandler(Counter counter)
        {
            _counter = counter;
        }

        public Task<RemoveInvoiceCommandResponse> Handle(RemoveInvoiceCommand request, CancellationToken cancellationToken)
        {
            _counter.Increment();
            RemoveInvoiceCommandResponse response = new RemoveInvoiceCommandResponse { Removed = true };
            return Task.FromResult(response);
        }
    }
}

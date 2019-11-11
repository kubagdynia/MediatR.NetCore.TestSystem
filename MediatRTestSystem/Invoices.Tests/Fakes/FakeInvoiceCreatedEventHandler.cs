using Invoices.Events;
using Kernel.Events;
using System.Threading.Tasks;
using System.Threading;

namespace Invoices.Tests.Fakes
{
    public class FakeInvoiceCreatedEventHandler : BaseEventHandler<InvoiceCreatedEvent>
    {
        private Counter _counter;

        public FakeInvoiceCreatedEventHandler(Counter counter)
        {
            _counter = counter;
        }

        protected override Task HandleEvent(InvoiceCreatedEvent domainEvent, CancellationToken cancellationToken)
        {
            _counter.Increment();
            return Task.CompletedTask;
        }
    }
}

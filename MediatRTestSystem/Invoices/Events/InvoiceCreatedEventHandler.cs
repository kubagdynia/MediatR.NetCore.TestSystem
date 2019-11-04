using Kernel.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Invoices.Events
{
    public class InvoiceCreatedEventHandler : BaseEventHandler<InvoiceCreatedEvent>
    {
        protected override Task HandleEvent(InvoiceCreatedEvent domainEvent, CancellationToken cancellationToken)
        {
            Console.WriteLine($"--------------------> Event received. Invoice created with id: {domainEvent.Id}");

            return Task.CompletedTask;
        }
    }
}

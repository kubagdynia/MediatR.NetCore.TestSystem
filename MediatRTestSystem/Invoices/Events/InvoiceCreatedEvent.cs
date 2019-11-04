using Kernel.Events;
using System;

namespace Invoices.Events
{
    public class InvoiceCreatedEvent : DomainEvent
    {
        public Guid Id { get; set; }

        public InvoiceCreatedEvent(Guid id)
        {
            Id = id;
        }
    }
}

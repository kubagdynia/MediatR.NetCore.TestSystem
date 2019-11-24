using Kernel.Events;
using System;

namespace Invoices.Events
{
    public class InvoiceCreatedEvent : DomainEvent
    {
        public Guid Id { get; set; }

        // To ensure correct deserialization
        protected InvoiceCreatedEvent()
        {

        }

        public InvoiceCreatedEvent(Guid id)
        {
            Id = id;
        }
    }
}

using Kernel;
using System;

namespace Invoices.Commands
{
    public class CreateInvoiceCommandResponse : BaseDomainResponse
    {
        public Guid Id { get; set; }
    }
}

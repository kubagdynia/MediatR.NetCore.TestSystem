using Kernel;

namespace Invoices.Commands
{
    public class RemoveInvoiceCommandResponse : BaseDomainResponse
    {
        public bool Removed { get; set; }
    }
}

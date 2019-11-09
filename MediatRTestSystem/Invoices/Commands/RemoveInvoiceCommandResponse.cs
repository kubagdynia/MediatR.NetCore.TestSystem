using Kernel.Responses.Domain;

namespace Invoices.Commands
{
    public class RemoveInvoiceCommandResponse : BaseDomainResponse
    {
        public bool Removed { get; set; }
    }
}

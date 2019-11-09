using Invoices.Models;
using Kernel.Responses.Domain;

namespace Invoices.Queries
{
    public class GetInvoiceQueryResponse : BaseDomainResponse
    {
        public Invoice? Invoice { get; set; }
    }
}

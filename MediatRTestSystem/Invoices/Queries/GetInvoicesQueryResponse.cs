using Invoices.Models;
using Kernel.Responses.Domain;
using System.Collections.Generic;

namespace Invoices.Queries
{
    public class GetInvoicesQueryResponse : BaseDomainResponse
    {
        public IEnumerable<Invoice>? Invoices { get; set; }
    }
}

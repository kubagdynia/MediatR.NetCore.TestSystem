using Invoices.Models;
using Kernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Invoices.Queries
{
    public class GetInvoicesQueryResponse : BaseDomainResponse
    {
        public IEnumerable<Invoice>? Invoices { get; set; }
    }
}

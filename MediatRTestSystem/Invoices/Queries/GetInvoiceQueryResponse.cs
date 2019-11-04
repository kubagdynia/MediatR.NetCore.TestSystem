using Invoices.Models;
using Kernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Invoices.Queries
{
    public class GetInvoiceQueryResponse : BaseDomainResponse
    {
        public Invoice? Invoice { get; set; }
    }
}

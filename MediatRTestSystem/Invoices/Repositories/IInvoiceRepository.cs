using Invoices.Models;
using System;
using System.Collections.Generic;

namespace Invoices.Repositories
{
    public interface IInvoiceRepository
    {
        IEnumerable<Invoice> Get();

        Invoice Get(Guid id);

        Guid Create(Invoice invoice);

        bool Remove(Guid id);
    }
}

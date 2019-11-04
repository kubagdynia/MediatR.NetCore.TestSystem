using Invoices.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Invoices.Repositories
{
    public class InvoiceMemoryRepository : IInvoiceRepository
    {
        private static readonly List<Invoice> Invoices = new List<Invoice>
        {
            new Invoice{Id = Guid.NewGuid(), Number = "1/1/2019", CreationDate = DateTime.UtcNow },
            new Invoice{Id = Guid.NewGuid(), Number = "1/2/2019", CreationDate = DateTime.UtcNow }
        };

        public Guid Create(Invoice invoice)
        {
            invoice.Id = Guid.NewGuid();
            Invoices.Add(invoice);

            return invoice.Id;
        }

        public IEnumerable<Invoice> Get()
        {
            return Invoices;
        }

        public Invoice Get(Guid id)
        {
            return Invoices.FirstOrDefault(c => c.Id == id);
        }

        public bool Remove(Guid id)
        {
            return Invoices.RemoveAll(c => c.Id == id) > 0;
        }
    }
}

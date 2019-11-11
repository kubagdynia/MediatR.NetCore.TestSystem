using Invoices.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Invoices.Repositories
{
    public class InvoiceMemoryRepository : IInvoiceRepository
    {
        private readonly List<Invoice> _invoices;

        public InvoiceMemoryRepository()
        {
            _invoices = new List<Invoice>();
        }

        public InvoiceMemoryRepository(List<Invoice> initialInvoicesData)
        {
            _invoices = initialInvoicesData;
        }

        public Guid Create(Invoice invoice)
        {
            _invoices.Add(invoice);
            return invoice.Id;
        }

        public IEnumerable<Invoice> Get()
        {
            return _invoices;
        }

        public Invoice Get(Guid id)
        {
            return _invoices.FirstOrDefault(c => c.Id == id);
        }

        public bool Remove(Guid id)
        {
            return _invoices.RemoveAll(c => c.Id == id) > 0;
        }
    }
}

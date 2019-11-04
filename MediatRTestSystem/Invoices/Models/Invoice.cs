using System;

namespace Invoices.Models
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public string? Number { get; set; }
        public DateTime CreationDate { get; set; }
    }
}

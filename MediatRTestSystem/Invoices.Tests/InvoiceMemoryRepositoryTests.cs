using FluentAssertions;
using Invoices.Repositories;
using NUnit.Framework;
using Invoices.Models;
using System;
using Bogus;
using System.Collections.Generic;

namespace Invoices.Tests
{
    [TestFixture]
    public class InvoiceMemoryRepositoryTests
    {
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void After_adding_a_certain_amount_of_invoices_the_repository_should_contain_the_same_amount_of_invoices(int count)
        {
            // Arrage
            IInvoiceRepository invoiceRepository = new InvoiceMemoryRepository();
            CreateRandomInvoices(invoiceRepository, count);

            // Act
            var invoices = invoiceRepository.Get();

            // Assert
            invoices.Should().HaveCount(count);
        }

        [Test]
        public void When_adds_three_invoices_and_delete_one_Repository_should_contain_two_invoices()
        {
            Guid invoiceId = Guid.NewGuid();

            // Arrage
            IInvoiceRepository invoiceRepository = new InvoiceMemoryRepository();
            CreateRandomInvoices(invoiceRepository, 2);
            invoiceRepository.Create(new Invoice(invoiceId, "J/03/2019", new DateTime(2019, 12, 05)));

            // Act
            var isRemoved = invoiceRepository.Remove(invoiceId);
            var invoices = invoiceRepository.Get();

            // Assert
            isRemoved.Should().BeTrue();
            invoices.Should().HaveCount(2);
        }

        [Test]
        public void Retrieving_an_invoice_that_was_previously_added_should_not_be_null()
        {
            Guid invoiceId = Guid.NewGuid();

            // Arrage
            IInvoiceRepository invoiceRepository = new InvoiceMemoryRepository();
            CreateRandomInvoices(invoiceRepository, 1);
            invoiceRepository.Create(new Invoice(invoiceId, "J/02/2019", new DateTime(2019, 11, 15)));
            CreateRandomInvoices(invoiceRepository, 1);

            // Act
            Invoice invoice = invoiceRepository.Get(invoiceId);

            // Assert
            invoice.Should().NotBeNull();
        }

        [Test]
        public void Retrieving_an_invoice_that_was_previously_added_should_be_the_same()
        {
            Invoice testInvoice = new Invoice(Guid.NewGuid(), "J/02/2019", new DateTime(2019, 11, 15));

            // Arrage
            IInvoiceRepository invoiceRepository = new InvoiceMemoryRepository();
            invoiceRepository.Create(testInvoice);
            CreateRandomInvoices(invoiceRepository, 2);

            // Act
            Invoice invoice = invoiceRepository.Get(testInvoice.Id);

            // Assert
            invoice.Should().BeEquivalentTo(testInvoice);
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void Invoices_should_be_correctly_added_from_the_constructor(int count)
        {
            // Arrage
            IInvoiceRepository invoiceRepository = new InvoiceMemoryRepository(CreateRandomInvoices(count));

            // Act
            var invoices = invoiceRepository.Get();

            // Assert
            invoices.Should().HaveCount(count);
        }

        public void CreateRandomInvoices(IInvoiceRepository invoiceRepository, int count)
        {
            foreach (Invoice invoice in CreateRandomInvoices(count))
            {
                invoiceRepository.Create(invoice);
            }
        }

        public List<Invoice> CreateRandomInvoices(int count)
        {
            var invoiceFaker = new Faker<Invoice>()
                .StrictMode(true)
                .RuleFor(c => c.Id, Guid.NewGuid())
                .RuleFor(c => c.Number, f => f.Random.String2(10))
                .RuleFor(c => c.CreationDate, f => f.Date.Recent(10));

            return invoiceFaker.Generate(count);
        }
    }
}

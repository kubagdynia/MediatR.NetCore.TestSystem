using Invoices.Commands;
using Invoices.Models;
using Invoices.Queries;
using Kernel.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class InvoicesController : BaseController
    {
        private readonly IMediator _mediator;

        public InvoicesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IEnumerable<Invoice>> Get()
        {
            GetInvoicesQueryResponse result = await _mediator.Send(new GetInvoicesQuery());
            return result.Invoices;
        }

        [HttpGet("{id}")]
        public async Task<Invoice> Get(Guid id)
        {
            GetInvoiceQueryResponse result = await _mediator.Send(new GetInvoiceQuery(id));
            return result.Invoice;
        }

        [HttpPost]
        public async Task<CreateInvoiceCommandResponse> Create([FromBody] Invoice invoice)
        {
            CreateInvoiceCommandResponse result = await _mediator.Send(new CreateInvoiceCommand(invoice));
            return result;
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            RemoveInvoiceCommandResponse result = await _mediator.Send(new RemoveInvoiceCommand(id));
            return result.Removed;
        }

    }
}

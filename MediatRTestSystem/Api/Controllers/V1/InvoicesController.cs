using Invoices.Commands;
using Invoices.Models;
using Invoices.Queries;
using Kernel.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers.V1
{
    [Route("api/v1/[controller]")]
    public class InvoicesController : BaseController
    {
        private readonly IMediator _mediator;

        public InvoicesController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Returns a list of all invoices
        /// </summary>
        /// <response code="200">Success - Returns a list of all invoices</response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Invoice>> Get()
        {
            GetInvoicesQueryResponse result = await _mediator.Send(new GetInvoicesQuery());
            return result.Invoices;
        }

        /// <summary>
        /// Returns the indicated invoice
        /// </summary>
        /// <param name="id">Invoice id</param>
        /// <response code="200">Success - Returns the invoice</response>
        /// <response code="204">No Content - No invoice found</response>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Invoice> Get(Guid id)
        {
            GetInvoiceQueryResponse result = await _mediator.Send(new GetInvoiceQuery(id));
            return result.Invoice;
        }

        /// <summary>
        /// Creates a new invoice
        /// </summary>
        /// <param name="invoice">Invoice data</param>
        /// <response code="200">Success - The invoice has been created</response>
        /// <response code="400">Bad Request - The invoice could not be created due to incorrect data</response>
        /// <returns>Id of invoice created</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CreateInvoiceCommandResponse), statusCode: 200)]
        public async Task<CreateInvoiceCommandResponse> Create([FromBody] Invoice invoice)
        {
            CreateInvoiceCommandResponse result = await _mediator.Send(new CreateInvoiceCommand(invoice));
            return result;
        }

        /// <summary>
        /// Deletes the indicated invoice
        /// </summary>
        /// <param name="id">Invoice id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            RemoveInvoiceCommandResponse result = await _mediator.Send(new RemoveInvoiceCommand(id));
            return result.Removed;
        }
    }
}

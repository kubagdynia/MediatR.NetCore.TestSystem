using Api.Contracts.V1.Requests;
using Api.Contracts.V1.Responses;
using Invoices.Commands;
using Invoices.Models;
using Invoices.Queries;
using Kernel.Controllers;
using Kernel.Messages;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers.V1
{
    [Route("api/v1/[controller]")]
    public class InvoicesController : BaseController
    {
        private readonly IMessageManager _messageManager;

        public InvoicesController(IMessageManager messageManager)
        {
            _messageManager = messageManager;
        }

        /// <summary>
        /// Returns a list of all invoices
        /// </summary>
        /// <response code="200">Success - Returns a list of all invoices</response>
        /// <response code="204">No Content - The are no invoices</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(type: typeof(InvoicesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(EmptyResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<InvoicesResponse>> GetInvoices()
        {
            GetInvoicesQueryResponse result = await _messageManager.SendCommand(new GetInvoicesQuery());

            if (result.Invoices is null || !result.Invoices.Any())
            {
                return NoContent();
            }

            var response = new InvoicesResponse(
                result.Invoices.Select(c => new InvoiceDto(c.Id, c.Number, c.CreationDate)),
                StatusCodes.Status200OK);             

            return Ok(response);
        }

        /// <summary>
        /// Returns the indicated invoice
        /// </summary>
        /// <response code="200">Success - Returns the invoice</response>
        /// <response code="404">Not Found - No invoice found</response>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(type: typeof(InvoiceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInvoice([FromRoute] GetInvoiceRequest query)
        {
            GetInvoiceQueryResponse result = await _messageManager.SendCommand(new GetInvoiceQuery(query.Id));

            if (result.Invoice is null)
            {
                return NotFound("Not found");
            }

            var response = new InvoiceResponse(
                new InvoiceDto(result.Invoice.Id, result.Invoice.Number, result.Invoice.CreationDate),
                StatusCodes.Status200OK);

            return Ok(response);
        }

        /// <summary>
        /// Creates a new invoice
        /// </summary>
        /// <param name="request">Invoice data</param>
        /// <response code="201">Success - The invoice has been created</response>
        /// <response code="400">Bad Request - The invoice could not be created due to incorrect data</response>
        /// <returns>Id of invoice created</returns>
        [HttpPost]
        [ProducesResponseType(type: typeof(CreateInvoiceResponse), statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
        {
            CreateInvoiceCommandResponse result = await _messageManager.SendCommand(
                new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: request.Number, creationDate: request.CreationDate)));

            return CreatedAtAction(nameof(GetInvoice), new GetInvoiceRequest { Id = result.Id },
                new CreateInvoiceResponse(result.Id, StatusCodes.Status201Created));
        }

        /// <summary>
        /// Deletes the indicated invoice
        /// </summary>
        /// <param name="id">Invoice id</param>
        /// <response code="204">No Content - The invoice has been deleted</response>
        /// <response code="404">Not Found - The invoice could not be found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            RemoveInvoiceCommandResponse result = await _messageManager.SendCommand(new RemoveInvoiceCommand(id));
            
            if (result.Removed)
            {
                return NoContent();
            }

            return NotFound("Not found");
        }
    }
}

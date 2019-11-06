using Api.Contracts.V1.Requests;
using Api.Contracts.V1.Responses;
using Invoices.Commands;
using Invoices.Models;
using Invoices.Queries;
using Kernel.Controllers;
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
        private readonly IMediator _mediator;

        public InvoicesController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Returns a list of all invoices
        /// </summary>
        /// <response code="200">Success - Returns a list of all invoices</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInvoices()
        {
            GetInvoicesQueryResponse result = await _mediator.Send(new GetInvoicesQuery());

            if (result.Invoices is null)
            {
                return NotFound();
            }

            var response = new BaseResponse<IEnumerable<InvoiceResponse>>(
                result.Invoices.Select(c => new InvoiceResponse(c.Id, c.Number, c.CreationDate)));

            return Ok(response);
        }

        /// <summary>
        /// Returns the indicated invoice
        /// </summary>
        /// <param name="query">Invoice id</param>
        /// <response code="200">Success - Returns the invoice</response>
        /// <response code="404">Not Found - No invoice found</response>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(type: typeof(BaseResponse<InvoiceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInvoice([FromRoute] GetInvoiceRequest query)
        {
            GetInvoiceQueryResponse result = await _mediator.Send(new GetInvoiceQuery(query.Id));

            if (result.Invoice is null)
            {
                return NotFound("Not found");
            }

            var response = new BaseResponse<InvoiceResponse>(
                new InvoiceResponse(result.Invoice.Id, result.Invoice.Number, result.Invoice.CreationDate));

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
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
        {
            CreateInvoiceCommandResponse result = await _mediator.Send(
                new CreateInvoiceCommand(new Invoice(id: Guid.NewGuid(), number: request.Number, creationDate: request.CreationDate)));

            return Ok(new BaseResponse<CreateInvoiceResponse>(new CreateInvoiceResponse(result.Id)));
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
            RemoveInvoiceCommandResponse result = await _mediator.Send(new RemoveInvoiceCommand(id));
            
            if (result.Removed)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}

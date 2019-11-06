using Microsoft.AspNetCore.Mvc;
using System;

namespace Api.Contracts.V1.Requests
{
    public class GetInvoiceRequest
    {
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
    }
}

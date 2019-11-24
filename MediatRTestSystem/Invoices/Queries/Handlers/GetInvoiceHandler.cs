using Invoices.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Invoices.Queries.Handlers
{
    public class GetInvoiceHandler : IRequestHandler<GetInvoiceQuery, GetInvoiceQueryResponse>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoiceHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public Task<GetInvoiceQueryResponse> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
        {
            GetInvoiceQueryResponse response = new GetInvoiceQueryResponse
            {
                Invoice = _invoiceRepository.Get(request.Id)
            };

            return Task.FromResult(response);
        }
    }
}

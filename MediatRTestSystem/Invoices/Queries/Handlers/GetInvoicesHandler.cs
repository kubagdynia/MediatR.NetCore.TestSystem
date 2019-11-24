using Invoices.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Invoices.Queries.Handlers
{
    public class GetInvoicesHandler : IRequestHandler<GetInvoicesQuery, GetInvoicesQueryResponse>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoicesHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public Task<GetInvoicesQueryResponse> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
        {
            GetInvoicesQueryResponse response = new GetInvoicesQueryResponse
            {
                Invoices = _invoiceRepository.Get()
            };

            return Task.FromResult(response);
        }
    }
}

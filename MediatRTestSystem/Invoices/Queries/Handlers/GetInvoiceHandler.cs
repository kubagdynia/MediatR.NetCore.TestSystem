using Invoices.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Invoices.Queries.Handlers
{
    public class GetInvoiceHandler : IRequestHandler<GetInvoiceQuery, GetInvoiceQueryResponse>
    {
        private readonly IInvoiceRepository invoiceRepository;

        public GetInvoiceHandler(IInvoiceRepository invoiceRepository)
        {
            this.invoiceRepository = invoiceRepository;
        }

        public Task<GetInvoiceQueryResponse> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
        {
            GetInvoiceQueryResponse response = new GetInvoiceQueryResponse
            {
                Invoice = invoiceRepository.Get(request.Id)
            };

            return Task.FromResult(response);
        }
    }
}

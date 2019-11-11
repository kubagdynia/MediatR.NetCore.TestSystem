using Invoices.Queries;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Invoices.Tests.Fakes
{
    public class FakeGetInvoiceHandler : IRequestHandler<GetInvoiceQuery, GetInvoiceQueryResponse>
    {
        private Counter _counter;

        public FakeGetInvoiceHandler(Counter counter)
        {
            _counter = counter;
        }

        public Task<GetInvoiceQueryResponse> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
        {
            _counter.Increment();
            GetInvoiceQueryResponse response = new GetInvoiceQueryResponse();
            return Task.FromResult(response);
        }
    }
}

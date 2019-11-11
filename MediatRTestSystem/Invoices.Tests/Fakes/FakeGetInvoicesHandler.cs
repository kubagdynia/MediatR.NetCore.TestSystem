using Invoices.Queries;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Invoices.Tests.Fakes
{
    public class FakeGetInvoicesHandler : IRequestHandler<GetInvoicesQuery, GetInvoicesQueryResponse>
    {
        private Counter _counter;

        public FakeGetInvoicesHandler(Counter counter)
        {
            _counter = counter;
        }

        public Task<GetInvoicesQueryResponse> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
        {
            _counter.Increment();
            GetInvoicesQueryResponse response = new GetInvoicesQueryResponse();
            return Task.FromResult(response);
        }
    }
}

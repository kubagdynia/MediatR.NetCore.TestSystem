using MediatR;

namespace Invoices.Queries
{
    public class GetInvoicesQuery : IRequest<GetInvoicesQueryResponse>
    {
    }
}

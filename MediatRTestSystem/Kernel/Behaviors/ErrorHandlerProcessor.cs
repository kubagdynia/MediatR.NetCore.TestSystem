using Kernel.Exceptions;
using MediatR.Pipeline;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kernel.Behaviors
{
    public class ErrorHandlerProcessor<TQuery, TResponse> : IRequestPostProcessor<TQuery, TResponse> where TResponse : BaseDomainResponse, new()
    {
        public Task Process(TQuery request, TResponse response, CancellationToken cancellationToken)
        {
            if (response.Errors != null && response.Errors.Any())
            {
                ThrowValidationException(response);
            }

            return Task.FromResult(response);
        }

        private static void ThrowValidationException(TResponse response)
        {
            var exception = new DomainException("Error");

            if (response.Errors == null)
            {
                return;
            }

            foreach (string error in response.Errors)
            {
                exception.AddDomainError(string.Empty, error, string.Empty, string.Empty);
            }

            throw exception;
        }
    }
}

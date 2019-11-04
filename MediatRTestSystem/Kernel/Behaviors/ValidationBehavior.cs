using FluentValidation;
using FluentValidation.Results;
using Kernel.Exceptions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kernel.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TResponse : BaseDomainResponse, new()
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);

            IEnumerable<ValidationFailure> errors = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (errors.Any())
            {
                ThrowValidationException(errors);
            }

            return next();
        }

        private static void ThrowValidationException(IEnumerable<ValidationFailure> errors)
        {
            var exception = new DomainException("Validation Error");

            foreach (ValidationFailure error in errors)
            {
                exception.AddDomainError(error.ErrorCode, error.ErrorMessage, error.PropertyName, error.AttemptedValue, GetErrorType(error.Severity));
            }

            throw exception;
        }

        private static DomainErrorType GetErrorType(Severity severity)
        {
            var errorType = severity switch
            {
                Severity.Error => DomainErrorType.Error,
                Severity.Warning => DomainErrorType.Warning,
                Severity.Info => DomainErrorType.Info,
                _ => DomainErrorType.Error,
            };
            return errorType;
        }
    }
}

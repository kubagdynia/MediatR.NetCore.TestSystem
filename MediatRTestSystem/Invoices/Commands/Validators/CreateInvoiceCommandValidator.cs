using FluentValidation;
using System;

namespace Invoices.Commands.Validators
{
    public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInvoiceCommandValidator()
        {
            RuleFor(c => c.Invoice.Number).Length(5, 20);
            RuleFor(c => c.Invoice.CreationDate).GreaterThan(new DateTime(2019, 01, 01));//.WithMessage("{PropertyName} should be greather than {2019.10.01}");
        }
    }
}

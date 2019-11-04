using FluentValidation;

namespace Invoices.Commands.Validators
{
    public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInvoiceCommandValidator()
        {
            RuleFor(c => c.Invoice.Number).Length(5, 20);
        }
    }
}

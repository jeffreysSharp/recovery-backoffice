using FluentValidation;
using Recovery.BackOffice.Modules.Billing.Contracts.v1.Invoices;

namespace Recovery.BackOffice.Modules.Billing.Features.v1.Invoices.GenerateInvoices;

public sealed class GenerateInvoicesCommandValidator : AbstractValidator<GenerateInvoicesCommand>
{
    public GenerateInvoicesCommandValidator()
    {
        RuleFor(x => x.PeriodYear).InclusiveBetween(2000, 2100);
        RuleFor(x => x.PeriodMonth).InclusiveBetween(1, 12);
    }
}

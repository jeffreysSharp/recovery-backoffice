using FluentValidation;
using Recovery.BackOffice.Modules.Billing.Contracts.v1.Wallets;

namespace Recovery.BackOffice.Modules.Billing.Features.v1.Wallets.ApproveTopupRequest;

public sealed class ApproveTopupRequestCommandValidator : AbstractValidator<ApproveTopupRequestCommand>
{
    public ApproveTopupRequestCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

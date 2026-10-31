using FluentValidation;
using Recovery.BackOffice.Modules.Identity.Contracts.v1.TwoFactor;

namespace Recovery.BackOffice.Modules.Identity.Features.v1.TwoFactor.Disable;

public sealed class DisableTwoFactorCommandValidator : AbstractValidator<DisableTwoFactorCommand>
{
    public DisableTwoFactorCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
    }
}

using FluentValidation;
using Recovery.BackOffice.Modules.Identity.Contracts.v1.Users.ForgotPassword;

namespace Recovery.BackOffice.Modules.Identity.Features.v1.Users.ForgotPassword;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(p => p.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();
    }
}
using FluentValidation;
using Recovery.BackOffice.Modules.Identity.Contracts.v1.Users.ResetPassword;

namespace Recovery.BackOffice.Modules.Identity.Features.v1.Users.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Token).NotEmpty();
    }
}
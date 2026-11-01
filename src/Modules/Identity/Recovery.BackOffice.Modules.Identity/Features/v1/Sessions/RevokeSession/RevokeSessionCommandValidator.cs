using FluentValidation;
using Recovery.BackOffice.Modules.Identity.Contracts.v1.Sessions.RevokeSession;

namespace Recovery.BackOffice.Modules.Identity.Features.v1.Sessions.RevokeSession;

public sealed class RevokeSessionCommandValidator : AbstractValidator<RevokeSessionCommand>
{
    public RevokeSessionCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");
    }
}
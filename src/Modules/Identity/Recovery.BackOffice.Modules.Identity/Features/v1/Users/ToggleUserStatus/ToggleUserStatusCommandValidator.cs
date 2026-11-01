using FluentValidation;
using Recovery.BackOffice.Modules.Identity.Contracts.v1.Users.ToggleUserStatus;

namespace Recovery.BackOffice.Modules.Identity.Features.v1.Users.ToggleUserStatus;

public sealed class ToggleUserStatusCommandValidator : AbstractValidator<ToggleUserStatusCommand>
{
    public ToggleUserStatusCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
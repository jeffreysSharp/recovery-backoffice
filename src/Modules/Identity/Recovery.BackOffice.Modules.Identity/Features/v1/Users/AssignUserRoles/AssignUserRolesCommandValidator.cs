using FluentValidation;
using Recovery.BackOffice.Modules.Identity.Contracts.v1.Users.AssignUserRoles;

namespace Recovery.BackOffice.Modules.Identity.Features.v1.Users.AssignUserRoles;

public sealed class AssignUserRolesCommandValidator : AbstractValidator<AssignUserRolesCommand>
{
    public AssignUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.UserRoles)
            .NotNull().WithMessage("User roles list is required.");
    }
}
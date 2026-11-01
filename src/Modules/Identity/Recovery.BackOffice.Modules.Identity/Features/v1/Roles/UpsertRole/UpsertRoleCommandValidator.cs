using FluentValidation;
using Recovery.BackOffice.Modules.Identity.Contracts.v1.Roles.UpsertRole;

namespace Recovery.BackOffice.Modules.Identity.Features.v1.Roles.UpsertRole;

public sealed class UpsertRoleCommandValidator : AbstractValidator<UpsertRoleCommand>
{
    public UpsertRoleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Role name is required.");
    }
}
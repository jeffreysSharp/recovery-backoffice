using FluentValidation;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.ChangeTenantActivation;

namespace Recovery.BackOffice.Modules.Multitenancy.Features.v1.ChangeTenantActivation;

internal sealed class ChangeTenantActivationCommandValidator : AbstractValidator<ChangeTenantActivationCommand>
{
    public ChangeTenantActivationCommandValidator() =>
       RuleFor(t => t.TenantId)
           .NotEmpty();
}
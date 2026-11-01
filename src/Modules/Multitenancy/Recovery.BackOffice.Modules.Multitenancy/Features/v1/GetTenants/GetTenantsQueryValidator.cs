using FluentValidation;
using Recovery.BackOffice.BuildingBlocks.Web.Validation;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.GetTenants;

namespace Recovery.BackOffice.Modules.Multitenancy.Features.v1.GetTenants;

public sealed class GetTenantsQueryValidator : AbstractValidator<GetTenantsQuery>
{
    public GetTenantsQueryValidator()
    {
        Include(new PagedQueryValidator<GetTenantsQuery>());
    }
}
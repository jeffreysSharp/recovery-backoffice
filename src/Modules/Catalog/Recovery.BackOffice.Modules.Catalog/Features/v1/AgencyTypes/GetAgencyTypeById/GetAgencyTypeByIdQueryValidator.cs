using FluentValidation;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.GetAgencyTypeById;
public sealed class GetAgencyTypeByIdQueryValidator : AbstractValidator<GetAgencyTypeByIdQuery>
{
    public GetAgencyTypeByIdQueryValidator() => RuleFor(request => request.AgencyTypeId).NotEmpty();
}

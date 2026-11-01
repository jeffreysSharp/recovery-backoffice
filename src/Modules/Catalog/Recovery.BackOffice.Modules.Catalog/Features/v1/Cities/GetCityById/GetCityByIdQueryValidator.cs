using FluentValidation;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.GetCityById;
public sealed class GetCityByIdQueryValidator : AbstractValidator<GetCityByIdQuery>
{
    public GetCityByIdQueryValidator() => RuleFor(request => request.CityId).NotEmpty();
}

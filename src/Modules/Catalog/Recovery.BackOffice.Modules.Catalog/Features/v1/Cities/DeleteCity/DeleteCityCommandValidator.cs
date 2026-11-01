using FluentValidation;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.DeleteCity;
public sealed class DeleteCityCommandValidator : AbstractValidator<DeleteCityCommand>
{
    public DeleteCityCommandValidator() => RuleFor(request => request.CityId).NotEmpty();
}

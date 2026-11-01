using FluentValidation;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.SearchCities;
public sealed class SearchCitiesQueryValidator : AbstractValidator<SearchCitiesQuery>
{
    public SearchCitiesQueryValidator()
    {
        RuleFor(query => query.PageNumber).GreaterThan(0);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 200);
        RuleFor(query => query.Search).MaximumLength(150);
    }
}

using FluentValidation;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.SearchCountries;

public sealed class SearchCountriesQueryValidator : AbstractValidator<SearchCountriesQuery>
{
    public SearchCountriesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.Search).MaximumLength(150);
    }
}

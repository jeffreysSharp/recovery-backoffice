using FluentValidation;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.SearchAgencyTypes;
public sealed class SearchAgencyTypesQueryValidator : AbstractValidator<SearchAgencyTypesQuery>
{
    public SearchAgencyTypesQueryValidator()
    {
        RuleFor(query => query.PageNumber).GreaterThan(0);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 200);
        RuleFor(query => query.Search).MaximumLength(500);
    }
}

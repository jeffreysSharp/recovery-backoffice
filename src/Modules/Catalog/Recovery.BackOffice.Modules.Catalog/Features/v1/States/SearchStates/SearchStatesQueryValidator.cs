using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;
using Recovery.BackOffice.Modules.Catalog.Data;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.SearchStates;

public sealed class SearchStatesQueryValidator : AbstractValidator<SearchStatesQuery>
{
    public SearchStatesQueryValidator()
    {

        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.Search).MaximumLength(100);

    }
}

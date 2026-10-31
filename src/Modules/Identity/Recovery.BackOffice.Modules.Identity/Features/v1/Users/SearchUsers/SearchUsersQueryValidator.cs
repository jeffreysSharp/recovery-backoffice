using FluentValidation;
using Recovery.BackOffice.BuildingBlocks.Web.Validation;
using Recovery.BackOffice.Modules.Identity.Contracts.v1.Users.SearchUsers;

namespace Recovery.BackOffice.Modules.Identity.Features.v1.Users.SearchUsers;

public sealed class SearchUsersQueryValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersQueryValidator()
    {
        Include(new PagedQueryValidator<SearchUsersQuery>());

        RuleFor(q => q.Search)
            .MaximumLength(200)
            .When(q => !string.IsNullOrEmpty(q.Search));

        RuleFor(q => q.RoleId)
            .MaximumLength(450)
            .When(q => !string.IsNullOrEmpty(q.RoleId));
    }
}
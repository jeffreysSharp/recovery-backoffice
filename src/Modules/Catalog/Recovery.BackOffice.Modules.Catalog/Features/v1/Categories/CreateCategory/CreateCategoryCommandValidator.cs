using FluentValidation;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Categories;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Categories.CreateCategory;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Description).MaximumLength(1024);
    }
}

using FluentValidation;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Products.AddProductImage;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Products.AddProductImage;

public sealed class AddProductImageCommandValidator : AbstractValidator<AddProductImageCommand>
{
    public AddProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Url).NotEmpty().MaximumLength(2048);
    }
}

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;
using Recovery.BackOffice.Modules.Catalog.Data;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.DeleteCountry;

public sealed class DeleteCountryCommandValidator : AbstractValidator<DeleteCountryCommand>
{
    public DeleteCountryCommandValidator()
    {
        RuleFor(x => x.CountryId).NotEmpty().WithMessage("Identificador é obrigatório.");

    }
}

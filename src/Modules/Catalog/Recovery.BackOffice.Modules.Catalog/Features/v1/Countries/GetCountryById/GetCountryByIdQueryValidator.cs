using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;
using Recovery.BackOffice.Modules.Catalog.Data;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.GetCountryById;

public sealed class GetCountryByIdQueryValidator : AbstractValidator<GetCountryByIdQuery>
{
    public GetCountryByIdQueryValidator()
    {
        RuleFor(x => x.CountryId).NotEmpty().WithMessage("Identificador é obrigatório.");

    }
}

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;
using Recovery.BackOffice.Modules.Catalog.Data;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.UpdateCountry;

public sealed class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
{
    public UpdateCountryCommandValidator(CountriesDbContext db)
    {
        RuleFor(x => x.CountryId).NotEmpty().WithMessage("Identificador é obrigatório.");
        RuleFor(x => x.Name).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("País é obrigatório.").MaximumLength(150).WithMessage("País deve ter no máximo 150 caracteres.");
        RuleFor(x => x.Iso2Code).NotEmpty().WithMessage("Código ISO 2 é obrigatório.")
            .Must(x => x is not null && System.Text.RegularExpressions.Regex.IsMatch(x.Trim(), "^[A-Za-z]{2}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100))).WithMessage("Código ISO 2 deve conter exatamente 2 letras.")
            .MustAsync(async (command, code, ct) =>
            {
                var normalized = code.Trim().ToUpperInvariant();
                return !await db.Countries.AnyAsync(x => x.Iso2Code == normalized && x.Id != command.CountryId, ct).ConfigureAwait(false);
            })
            .When(x => !string.IsNullOrWhiteSpace(x.Iso2Code), ApplyConditionTo.CurrentValidator).WithMessage("Código ISO 2 já cadastrado.");
        RuleFor(x => x.Iso3Code).NotEmpty().WithMessage("Código ISO 3 é obrigatório.")
            .Must(x => x is not null && System.Text.RegularExpressions.Regex.IsMatch(x.Trim(), "^[A-Za-z]{3}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100))).WithMessage("Código ISO 3 deve conter exatamente 3 letras.")
            .MustAsync(async (command, code, ct) =>
            {
                var normalized = code.Trim().ToUpperInvariant();
                return !await db.Countries.AnyAsync(x => x.Iso3Code == normalized && x.Id != command.CountryId, ct).ConfigureAwait(false);
            })
            .When(x => !string.IsNullOrWhiteSpace(x.Iso3Code), ApplyConditionTo.CurrentValidator).WithMessage("Código ISO 3 já cadastrado.");
        RuleFor(x => x.NumericCode).Must(x => string.IsNullOrWhiteSpace(x) || System.Text.RegularExpressions.Regex.IsMatch(x.Trim(), "^[0-9]{3}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100))).WithMessage("Código numérico deve conter exatamente 3 dígitos.");
        RuleFor(x => x.PhoneCode).Must(x => string.IsNullOrWhiteSpace(x) || System.Text.RegularExpressions.Regex.IsMatch(x.Trim(), @"^\+[1-9][0-9]{0,3}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100))).WithMessage("Código telefônico deve conter + e de 1 a 4 dígitos.");
    }
}

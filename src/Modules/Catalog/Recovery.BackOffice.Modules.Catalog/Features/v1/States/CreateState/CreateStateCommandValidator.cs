using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;
using Recovery.BackOffice.Modules.Catalog.Data;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.CreateState;

public sealed class CreateStateCommandValidator : AbstractValidator<CreateStateCommand>
{
    public CreateStateCommandValidator(CountriesDbContext db)
    {

        RuleFor(x => x.CountryId).NotEmpty().WithMessage("País é obrigatório.")
            .MustAsync((id, ct) => db.Countries.AnyAsync(country => country.Id == id, ct)).WithMessage("País não encontrado.");
        RuleFor(x => x.Name).Must(name => !string.IsNullOrWhiteSpace(name) && name.Trim().Length is >= 2 and <= 100)
            .WithMessage("Estado deve ter entre 2 e 100 caracteres.");
        RuleFor(x => x.Code).Must(code => !string.IsNullOrWhiteSpace(code) && code.Trim().Length is >= 1 and <= 10)
            .WithMessage("Sigla deve ter entre 1 e 10 caracteres.")
            .MustAsync(async (command, code, ct) =>
            {
                var normalized = code.Trim().ToUpperInvariant();
                return !await db.States.AnyAsync(state => state.CountryId == command.CountryId && state.Code == normalized , ct).ConfigureAwait(false);
            }).When(x => !string.IsNullOrWhiteSpace(x.Code), ApplyConditionTo.CurrentValidator).WithMessage("Sigla já cadastrada para este país.");
        RuleFor(x => x.IbgeCode)
            .Must(code => string.IsNullOrWhiteSpace(code) || System.Text.RegularExpressions.Regex.IsMatch(code.Trim(), "^[0-9]{2}$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)))
            .WithMessage("Código IBGE deve conter exatamente 2 dígitos.")
            .MustAsync(async (command, code, ct) => string.IsNullOrWhiteSpace(code) ||
                await db.Countries.AnyAsync(country => country.Id == command.CountryId && country.Iso2Code == "BR", ct).ConfigureAwait(false))
            .WithMessage("Código IBGE é permitido somente para estados brasileiros.")
            .MustAsync(async (command, code, ct) =>
            {
                if (string.IsNullOrWhiteSpace(code)) return true;
                var normalized = code.Trim();
                return !await db.States.AnyAsync(state => state.CountryId == command.CountryId && state.IbgeCode == normalized , ct).ConfigureAwait(false);
            }).WithMessage("Código IBGE já cadastrado para este país.");

    }
}

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.CreateCity;
public sealed class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator(CountriesDbContext db)
    {
        
        RuleFor(command => command.StateId).NotEmpty().WithMessage("Estado é obrigatório.")
            .MustAsync((id, ct) => db.States.AnyAsync(state => state.Id == id, ct)).WithMessage("Estado não encontrado.");
        RuleFor(command => command.Name).Must(name => !string.IsNullOrWhiteSpace(name) && City.NormalizeName(name).Length is >= 2 and <= 150)
            .WithMessage("Cidade deve ter entre 2 e 150 caracteres.")
            .MustAsync(async (command, name, ct) =>
            {
                var normalized = City.NormalizeName(name);
                return !await db.Cities.AnyAsync(city => city.StateId == command.StateId && city.Name == normalized , ct).ConfigureAwait(false);
            }).When(command => !string.IsNullOrWhiteSpace(command.Name), ApplyConditionTo.CurrentValidator)
            .WithMessage("Cidade já cadastrada para este estado.");
        RuleFor(command => command.IbgeCode)
            .Must(code => string.IsNullOrWhiteSpace(code) || System.Text.RegularExpressions.Regex.IsMatch(code.Trim(), "^[0-9]{7}$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)))
            .WithMessage("Código IBGE deve conter exatamente 7 dígitos.")
            .MustAsync(async (command, code, ct) => !string.IsNullOrWhiteSpace(code) ||
                !await db.States.Join(db.Countries, state => state.CountryId, country => country.Id, (state, country) => new { state.Id, country.Iso2Code })
                    .AnyAsync(row => row.Id == command.StateId && row.Iso2Code == "BR", ct).ConfigureAwait(false))
            .WithMessage("Código IBGE é obrigatório para cidades brasileiras.")
            .MustAsync(async (command, code, ct) =>
            {
                if (string.IsNullOrWhiteSpace(code)) return true;
                var normalized = code.Trim();
                return !await db.Cities.AnyAsync(city => city.IbgeCode == normalized , ct).ConfigureAwait(false);
            }).WithMessage("Código IBGE já cadastrado.");
    }
}

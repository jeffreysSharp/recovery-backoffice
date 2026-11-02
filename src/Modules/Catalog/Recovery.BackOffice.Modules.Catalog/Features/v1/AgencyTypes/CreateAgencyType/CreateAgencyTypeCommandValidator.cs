using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.CreateAgencyType;
public sealed class CreateAgencyTypeCommandValidator : AbstractValidator<CreateAgencyTypeCommand>
{
    public CreateAgencyTypeCommandValidator(CountriesDbContext db)
    {
        
        RuleFor(command => command.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name) && AgencyType.NormalizeName(name).Length is >= 3 and <= 100)
            .WithMessage("Tipo de agência deve ter entre 3 e 100 caracteres.")
            .MustAsync(async (command, name, ct) =>
            {
                var normalized = AgencyType.NormalizeName(name);
                return !await db.AgencyTypes.AnyAsync(agencyType => agencyType.Name == normalized , ct).ConfigureAwait(false);
            }).When(command => !string.IsNullOrWhiteSpace(command.Name), ApplyConditionTo.CurrentValidator)
            .WithMessage("Tipo de agência já cadastrado.");
        RuleFor(command => command.Description)
            .Must(description => string.IsNullOrWhiteSpace(description) || description.Trim().Length <= 500)
            .WithMessage("Descrição deve ter no máximo 500 caracteres.");
    }
}

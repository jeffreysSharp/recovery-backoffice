using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.CreateCountry;

public sealed class CreateCountryCommandHandler(CountriesDbContext db) : ICommandHandler<CreateCountryCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateCountryCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var country = Country.Create(command.Name, command.Iso2Code, command.Iso3Code, command.NumericCode, command.PhoneCode, command.IsActive);
        db.Countries.Add(country);
        await CountryPersistence.SaveAsync(db, cancellationToken).ConfigureAwait(false);
        return country.Id;
    }
}

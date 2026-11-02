using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.UpdateCountry;

public sealed class UpdateCountryCommandHandler(CountriesDbContext db) : ICommandHandler<UpdateCountryCommand, Guid>
{
    public async ValueTask<Guid> Handle(UpdateCountryCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var country = await db.Countries.SingleOrDefaultAsync(x => x.Id == command.CountryId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("País não encontrado.");
        country.Update(command.Name, command.Iso2Code, command.Iso3Code, command.NumericCode, command.PhoneCode, command.IsActive);
        await CountryPersistence.SaveAsync(db, cancellationToken).ConfigureAwait(false);
        return country.Id;
    }
}

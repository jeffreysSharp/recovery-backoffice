using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.DeleteCountry;

public sealed class DeleteCountryCommandHandler(CountriesDbContext db) : ICommandHandler<DeleteCountryCommand, Unit>
{
    public async ValueTask<Unit> Handle(DeleteCountryCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var country = await db.Countries.SingleOrDefaultAsync(x => x.Id == command.CountryId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("País não encontrado.");
        db.Countries.Remove(country);
        await CountryPersistence.SaveAsync(db, cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}

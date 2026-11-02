using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.DeleteCity;

public sealed class DeleteCityCommandHandler(CountriesDbContext db) : ICommandHandler<DeleteCityCommand, Unit>
{
    public async ValueTask<Unit> Handle(DeleteCityCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var city = await db.Cities.SingleOrDefaultAsync(x => x.Id == command.CityId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Cidade não encontrada.");
        db.Cities.Remove(city);
        await CityPersistence.SaveAsync(db, cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}

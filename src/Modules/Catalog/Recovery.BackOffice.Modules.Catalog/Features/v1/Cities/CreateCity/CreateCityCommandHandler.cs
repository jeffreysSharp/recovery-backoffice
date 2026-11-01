using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.CreateCity;

public sealed class CreateCityCommandHandler(CountriesDbContext db) : ICommandHandler<CreateCityCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateCityCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var city = City.Create(command.StateId, command.Name, command.IbgeCode, command.IsActive);
        db.Cities.Add(city);
        await CityPersistence.SaveAsync(db, cancellationToken).ConfigureAwait(false);
        return city.Id;
    }
}

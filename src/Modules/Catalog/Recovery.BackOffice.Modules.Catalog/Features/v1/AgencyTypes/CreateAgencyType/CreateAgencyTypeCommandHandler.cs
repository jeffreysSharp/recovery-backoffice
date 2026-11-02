using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.CreateAgencyType;

public sealed class CreateAgencyTypeCommandHandler(CountriesDbContext db) : ICommandHandler<CreateAgencyTypeCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateAgencyTypeCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var agencyType = AgencyType.Create(command.Name, command.Description, command.IsActive);
        db.AgencyTypes.Add(agencyType);
        await AgencyTypePersistence.SaveAsync(db, cancellationToken).ConfigureAwait(false);
        return agencyType.Id;
    }
}

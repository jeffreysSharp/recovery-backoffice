using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.UpdateAgencyType;

public sealed class UpdateAgencyTypeCommandHandler(CountriesDbContext db) : ICommandHandler<UpdateAgencyTypeCommand, Guid>
{
    public async ValueTask<Guid> Handle(UpdateAgencyTypeCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var agencyType = await db.AgencyTypes.SingleOrDefaultAsync(x => x.Id == command.AgencyTypeId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Tipo de agência não encontrado.");
        agencyType.Update(command.Name, command.Description, command.IsActive);
        await AgencyTypePersistence.SaveAsync(db, cancellationToken).ConfigureAwait(false);
        return agencyType.Id;
    }
}

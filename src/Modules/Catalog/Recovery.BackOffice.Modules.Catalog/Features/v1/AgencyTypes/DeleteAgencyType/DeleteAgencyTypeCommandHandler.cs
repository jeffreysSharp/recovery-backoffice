using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.DeleteAgencyType;

public sealed class DeleteAgencyTypeCommandHandler(CountriesDbContext db) : ICommandHandler<DeleteAgencyTypeCommand, Unit>
{
    public async ValueTask<Unit> Handle(DeleteAgencyTypeCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var agencyType = await db.AgencyTypes.SingleOrDefaultAsync(x => x.Id == command.AgencyTypeId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Tipo de agência não encontrado.");
        db.AgencyTypes.Remove(agencyType);
        await AgencyTypePersistence.SaveAsync(db, cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}

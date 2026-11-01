using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.UpdateState;

public sealed class UpdateStateCommandHandler(CountriesDbContext db) : ICommandHandler<UpdateStateCommand, Guid>
{
    public async ValueTask<Guid> Handle(UpdateStateCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var state = await db.States.SingleOrDefaultAsync(x => x.Id == command.StateId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Estado não encontrado.");
        state.Update(command.CountryId, command.Name, command.Code, command.IbgeCode, command.IsActive);
        await StatePersistence.SaveAsync(db, cancellationToken).ConfigureAwait(false);
        return state.Id;
    }
}

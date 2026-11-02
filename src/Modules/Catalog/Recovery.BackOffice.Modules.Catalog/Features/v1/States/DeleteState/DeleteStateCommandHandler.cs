using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.DeleteState;

public sealed class DeleteStateCommandHandler(CountriesDbContext db) : ICommandHandler<DeleteStateCommand, Unit>
{
    public async ValueTask<Unit> Handle(DeleteStateCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var state = await db.States.SingleOrDefaultAsync(x => x.Id == command.StateId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Estado não encontrado.");
        db.States.Remove(state);
        await StatePersistence.SaveAsync(db, cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}

using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.CreateState;

public sealed class CreateStateCommandHandler(CountriesDbContext db) : ICommandHandler<CreateStateCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateStateCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var state = State.Create(command.CountryId, command.Name, command.Code, command.IbgeCode, command.IsActive);
        db.States.Add(state);
        await StatePersistence.SaveAsync(db, cancellationToken).ConfigureAwait(false);
        return state.Id;
    }
}

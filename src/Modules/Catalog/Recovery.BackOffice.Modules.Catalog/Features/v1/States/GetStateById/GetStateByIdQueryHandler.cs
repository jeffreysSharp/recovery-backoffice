using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;
using Recovery.BackOffice.Modules.Catalog.Data;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.GetStateById;

public sealed class GetStateByIdQueryHandler(CountriesDbContext db) : IQueryHandler<GetStateByIdQuery, StateDto>
{
    public async ValueTask<StateDto> Handle(GetStateByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        return await db.States.AsNoTracking().Where(state => state.Id == query.StateId)
            .Join(db.Countries.AsNoTracking(), state => state.CountryId, country => country.Id,
                (state, country) => new StateDto(state.Id, state.CountryId, country.Name, state.Name, state.Code, state.IbgeCode, state.IsActive))
            .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Estado não encontrado.");
    }
}

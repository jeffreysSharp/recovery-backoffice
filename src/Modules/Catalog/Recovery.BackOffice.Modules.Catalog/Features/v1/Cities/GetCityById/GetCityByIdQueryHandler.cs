using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;
using Recovery.BackOffice.Modules.Catalog.Data;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.GetCityById;
public sealed class GetCityByIdQueryHandler(CountriesDbContext db) : IQueryHandler<GetCityByIdQuery, CityDto>
{
    public async ValueTask<CityDto> Handle(GetCityByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        return await (from city in db.Cities.AsNoTracking()
            join state in db.States on city.StateId equals state.Id
            join country in db.Countries on state.CountryId equals country.Id
            where city.Id == query.CityId
            select new CityDto(city.Id, state.Id, state.Name, country.Id, country.Name, city.Name, city.IbgeCode, city.IsActive))
            .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false) ?? throw new NotFoundException("Cidade não encontrada.");
    }
}

using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.GetCountryById;

public sealed class GetCountryByIdQueryHandler(CountriesDbContext db) : IQueryHandler<GetCountryByIdQuery, CountryDto>
{
    public async ValueTask<CountryDto> Handle(GetCountryByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var country = await db.Countries.AsNoTracking().SingleOrDefaultAsync(x => x.Id == query.CountryId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("País não encontrado.");
        return new CountryDto(country.Id, country.Name, country.Iso2Code, country.Iso3Code, country.NumericCode, country.PhoneCode, country.IsActive);
    }
}

using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;
using Recovery.BackOffice.Modules.Catalog.Data;

using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.SearchCities;
public sealed class SearchCitiesQueryHandler(CountriesDbContext db) : IQueryHandler<SearchCitiesQuery, PagedResponse<CityDto>>
{
    public async ValueTask<PagedResponse<CityDto>> Handle(SearchCitiesQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        var cities = from city in db.Cities.AsNoTracking()
            join state in db.States on city.StateId equals state.Id
            join country in db.Countries on state.CountryId equals country.Id
            select new { City = city, State = state, Country = country };
        if (query.CountryId.HasValue) cities = cities.Where(row => row.Country.Id == query.CountryId.Value);
        if (query.StateId.HasValue) cities = cities.Where(row => row.State.Id == query.StateId.Value);
        if (query.IsActive.HasValue) cities = cities.Where(row => row.City.IsActive == query.IsActive.Value);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            cities = cities.Where(row => EF.Functions.Like(row.City.Name, $"%{term}%") || (row.City.IbgeCode != null && row.City.IbgeCode.Contains(term)));
        }
        bool desc = string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        var ordered = query.SortBy?.ToUpperInvariant() switch
        {
            "IBGECODE" => desc ? cities.OrderByDescending(row => row.City.IbgeCode) : cities.OrderBy(row => row.City.IbgeCode),
            "STATENAME" => desc ? cities.OrderByDescending(row => row.State.Name) : cities.OrderBy(row => row.State.Name),
            "COUNTRYNAME" => desc ? cities.OrderByDescending(row => row.Country.Name) : cities.OrderBy(row => row.Country.Name),
            "ISACTIVE" => desc ? cities.OrderByDescending(row => row.City.IsActive) : cities.OrderBy(row => row.City.IsActive),
            _ => desc ? cities.OrderByDescending(row => row.City.Name) : cities.OrderBy(row => row.City.Name)
        };
        long total = await cities.LongCountAsync(cancellationToken).ConfigureAwait(false);
        var items = await ordered.ThenBy(row => row.City.Id).Skip(PaginationExtensions.GetOffset(query.PageNumber, query.PageSize)).Take(query.PageSize)
            .Select(row => new CityDto(row.City.Id, row.State.Id, row.State.Name, row.Country.Id, row.Country.Name, row.City.Name, row.City.IbgeCode, row.City.IsActive))
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return new PagedResponse<CityDto> { Items = items, TotalCount = total, PageNumber = query.PageNumber, PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }
}

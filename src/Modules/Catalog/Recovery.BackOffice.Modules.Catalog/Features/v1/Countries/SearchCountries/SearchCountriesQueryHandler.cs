using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;
using Recovery.BackOffice.Modules.Catalog.Data;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.SearchCountries;

public sealed class SearchCountriesQueryHandler(CountriesDbContext db) : IQueryHandler<SearchCountriesQuery, PagedResponse<CountryDto>>
{
    public async ValueTask<PagedResponse<CountryDto>> Handle(SearchCountriesQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        var countries = db.Countries.AsNoTracking().AsQueryable();
        if (query.IsActive.HasValue) countries = countries.Where(x => x.IsActive == query.IsActive.Value);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            countries = countries.Where(x => EF.Functions.Like(EF.Functions.Collate(x.Name, "Latin1_General_100_CI_AS"), $"%{term}%")
                || EF.Functions.Like(EF.Functions.Collate(x.Iso2Code, "Latin1_General_100_CI_AS"), $"%{term}%")
                || EF.Functions.Like(EF.Functions.Collate(x.Iso3Code, "Latin1_General_100_CI_AS"), $"%{term}%"));
        }
        bool desc = string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        var ordered = query.SortBy?.ToUpperInvariant() switch
        {
            "ISO2CODE" => desc ? countries.OrderByDescending(x => x.Iso2Code) : countries.OrderBy(x => x.Iso2Code),
            "ISO3CODE" => desc ? countries.OrderByDescending(x => x.Iso3Code) : countries.OrderBy(x => x.Iso3Code),
            "NUMERICCODE" => desc ? countries.OrderByDescending(x => x.NumericCode) : countries.OrderBy(x => x.NumericCode),
            "PHONECODE" => desc ? countries.OrderByDescending(x => x.PhoneCode) : countries.OrderBy(x => x.PhoneCode),
            "ISACTIVE" => desc ? countries.OrderByDescending(x => x.IsActive) : countries.OrderBy(x => x.IsActive),
            _ => desc ? countries.OrderByDescending(x => x.Name) : countries.OrderBy(x => x.Name)
        };
        long total = await countries.LongCountAsync(cancellationToken).ConfigureAwait(false);
        var items = await ordered.ThenBy(x => x.Id).Skip(PaginationExtensions.GetOffset(query.PageNumber, query.PageSize))
            .Take(query.PageSize).Select(x => new CountryDto(x.Id, x.Name, x.Iso2Code, x.Iso3Code, x.NumericCode, x.PhoneCode, x.IsActive)).ToListAsync(cancellationToken).ConfigureAwait(false);
        return new PagedResponse<CountryDto> { Items = items, PageNumber = query.PageNumber, PageSize = query.PageSize,
            TotalCount = total, TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }
}

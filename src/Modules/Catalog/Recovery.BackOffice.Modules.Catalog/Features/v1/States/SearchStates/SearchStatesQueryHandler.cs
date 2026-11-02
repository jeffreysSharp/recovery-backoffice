using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;
using Recovery.BackOffice.Modules.Catalog.Data;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.SearchStates;

public sealed class SearchStatesQueryHandler(CountriesDbContext db) : IQueryHandler<SearchStatesQuery, PagedResponse<StateDto>>
{
    public async ValueTask<PagedResponse<StateDto>> Handle(SearchStatesQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        var states = db.States.AsNoTracking().Join(db.Countries.AsNoTracking(), state => state.CountryId, country => country.Id,
            (state, country) => new { Entity = state, CountryName = country.Name });
        if (query.CountryId.HasValue) states = states.Where(state => state.Entity.CountryId == query.CountryId.Value);
        if (query.IsActive.HasValue) states = states.Where(state => state.Entity.IsActive == query.IsActive.Value);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            states = states.Where(state => EF.Functions.Like(EF.Functions.Collate(state.Entity.Name, "Latin1_General_100_CI_AS"), $"%{term}%")
                || EF.Functions.Like(EF.Functions.Collate(state.Entity.Code, "Latin1_General_100_CI_AS"), $"%{term}%")
                || (state.Entity.IbgeCode != null && state.Entity.IbgeCode.Contains(term)));
        }
        bool desc = string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        var ordered = query.SortBy?.ToUpperInvariant() switch
        {
            "CODE" => desc ? states.OrderByDescending(state => state.Entity.Code) : states.OrderBy(state => state.Entity.Code),
            "IBGECODE" => desc ? states.OrderByDescending(state => state.Entity.IbgeCode) : states.OrderBy(state => state.Entity.IbgeCode),
            "COUNTRYNAME" => desc ? states.OrderByDescending(state => state.CountryName) : states.OrderBy(state => state.CountryName),
            "ISACTIVE" => desc ? states.OrderByDescending(state => state.Entity.IsActive) : states.OrderBy(state => state.Entity.IsActive),
            _ => desc ? states.OrderByDescending(state => state.Entity.Name) : states.OrderBy(state => state.Entity.Name)
        };
        long total = await states.LongCountAsync(cancellationToken).ConfigureAwait(false);
        var items = await ordered.ThenBy(state => state.Entity.Id).Skip(PaginationExtensions.GetOffset(query.PageNumber, query.PageSize))
            .Take(query.PageSize).Select(state => new StateDto(state.Entity.Id, state.Entity.CountryId, state.CountryName, state.Entity.Name, state.Entity.Code, state.Entity.IbgeCode, state.Entity.IsActive)).ToListAsync(cancellationToken).ConfigureAwait(false);
        return new PagedResponse<StateDto> { Items = items, PageNumber = query.PageNumber, PageSize = query.PageSize,
            TotalCount = total, TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }
}

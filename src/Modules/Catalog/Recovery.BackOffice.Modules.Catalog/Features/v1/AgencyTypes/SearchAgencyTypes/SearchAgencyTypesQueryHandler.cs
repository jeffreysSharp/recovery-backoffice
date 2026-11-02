using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
using Recovery.BackOffice.Modules.Catalog.Data;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.SearchAgencyTypes;
public sealed class SearchAgencyTypesQueryHandler(CountriesDbContext db) : IQueryHandler<SearchAgencyTypesQuery, PagedResponse<AgencyTypeDto>>
{
    public async ValueTask<PagedResponse<AgencyTypeDto>> Handle(SearchAgencyTypesQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        var agencyTypes = db.AgencyTypes.AsNoTracking().AsQueryable();
        if (query.IsActive.HasValue) agencyTypes = agencyTypes.Where(agencyType => agencyType.IsActive == query.IsActive.Value);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            agencyTypes = agencyTypes.Where(agencyType => EF.Functions.Like(agencyType.Name, $"%{term}%") ||
                (agencyType.Description != null && EF.Functions.Like(EF.Functions.Collate(agencyType.Description, "Latin1_General_100_CI_AS"), $"%{term}%")));
        }
        bool desc = string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        var ordered = query.SortBy?.ToUpperInvariant() switch
        {
            "DESCRIPTION" => desc ? agencyTypes.OrderByDescending(agencyType => agencyType.Description) : agencyTypes.OrderBy(agencyType => agencyType.Description),
            "ISACTIVE" => desc ? agencyTypes.OrderByDescending(agencyType => agencyType.IsActive) : agencyTypes.OrderBy(agencyType => agencyType.IsActive),
            _ => desc ? agencyTypes.OrderByDescending(agencyType => agencyType.Name) : agencyTypes.OrderBy(agencyType => agencyType.Name)
        };
        long total = await agencyTypes.LongCountAsync(cancellationToken).ConfigureAwait(false);
        var items = await ordered.ThenBy(agencyType => agencyType.Id).Skip(PaginationExtensions.GetOffset(query.PageNumber, query.PageSize))
            .Take(query.PageSize).Select(agencyType => new AgencyTypeDto(agencyType.Id, agencyType.Name, agencyType.Description, agencyType.IsActive))
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return new PagedResponse<AgencyTypeDto> { Items = items, TotalCount = total, PageNumber = query.PageNumber, PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }
}

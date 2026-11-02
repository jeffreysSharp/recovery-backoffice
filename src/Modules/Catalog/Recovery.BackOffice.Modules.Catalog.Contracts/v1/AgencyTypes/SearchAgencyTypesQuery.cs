using Mediator;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
public sealed record SearchAgencyTypesQuery(string? Search = null, bool? IsActive = null, int PageNumber = 1, int PageSize = 20,
    string? SortBy = null, string? SortDir = null) : IQuery<PagedResponse<AgencyTypeDto>>;

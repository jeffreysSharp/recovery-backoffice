using Mediator;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;
public sealed record SearchCitiesQuery(string? Search = null, Guid? CountryId = null, Guid? StateId = null, bool? IsActive = null,
    int PageNumber = 1, int PageSize = 20, string? SortBy = null, string? SortDir = null) : IQuery<PagedResponse<CityDto>>;

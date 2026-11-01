using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Categories;

public sealed record ListTrashedCategoriesQuery(int PageNumber = 1, int PageSize = 20)
    : IQuery<PagedResponse<CategoryDto>>;

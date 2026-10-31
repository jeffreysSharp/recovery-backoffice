using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Products;
using Recovery.BackOffice.Modules.Catalog.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Products.ListTrashedProducts;

public sealed class ListTrashedProductsQueryHandler(CatalogDbContext dbContext)
    : IQueryHandler<ListTrashedProductsQuery, PagedResponse<ProductDto>>
{
    public async ValueTask<PagedResponse<ProductDto>> Handle(
        ListTrashedProductsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        int page = query.PageNumber < 1 ? 1 : query.PageNumber;
        int size = query.PageSize is < 1 or > 200 ? 20 : query.PageSize;

        var q = dbContext.Products
            .AsNoTracking()
            .IgnoreQueryFilters([QueryFilters.SoftDelete])
            .Where(p => p.IsDeleted)
            .OrderByDescending(p => p.DeletedOnUtc);

        long total = await q.LongCountAsync(cancellationToken).ConfigureAwait(false);
        var products = await q
            .Skip(PaginationExtensions.GetOffset(page, size))
            .Take(size)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResponse<ProductDto>
        {
            Items = products.Select(p => p.ToDto()).ToList(),
            PageNumber = page,
            PageSize = size,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)size),
        };
    }
}

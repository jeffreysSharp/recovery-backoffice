using Finbuckle.MultiTenant.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Billing.Contracts.Dtos;
using Recovery.BackOffice.Modules.Billing.Contracts.v1.Wallets;
using Recovery.BackOffice.Modules.Billing.Data;
using Recovery.BackOffice.Modules.Billing.Mappings;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Billing.Features.v1.Wallets.GetMyTopupRequests;

public sealed class GetMyTopupRequestsQueryHandler(
    BillingDbContext dbContext,
    IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    : IQueryHandler<GetMyTopupRequestsQuery, PagedResponse<TopupRequestDto>>
{
    public async ValueTask<PagedResponse<TopupRequestDto>> Handle(GetMyTopupRequestsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        // BillingDbContext is not tenant-filtered; resolve caller's own tenant and scope strictly to it.
        var tenantId = tenantAccessor.MultiTenantContext?.TenantInfo?.Id
            ?? throw new UnauthorizedException("Agency context is required.");

        var q = dbContext.TopupRequests.AsNoTracking()
            .Where(r => r.TenantId == tenantId);

        if (query.Status is not null)
        {
            q = q.Where(r => r.Status == query.Status);
        }

        var total = await q.LongCountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
            .OrderByDescending(r => r.CreatedAtUtc)
            .Skip(PaginationExtensions.GetOffset(query.PageNumber, query.PageSize))
            .Take(query.PageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return new PagedResponse<TopupRequestDto>
        {
            Items = items.Select(r => r.ToDto()).ToList(),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)query.PageSize)
        };
    }
}

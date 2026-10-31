using Finbuckle.MultiTenant.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Modules.Billing.Contracts.Dtos;
using Recovery.BackOffice.Modules.Billing.Contracts.v1.Invoices;
using Recovery.BackOffice.Modules.Billing.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Billing.Features.v1.Invoices.GetInvoiceById;

public sealed class GetInvoiceByIdQueryHandler(
    BillingDbContext dbContext,
    IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    : IQueryHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    public async ValueTask<InvoiceDto> Handle(GetInvoiceByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        // BillingDbContext isn't tenant-filtered (raw DbContext for cross-tenant admin visibility): root
        // reads any invoice by id; a tenant caller is pinned to its own so it can't read another's. Mirrors GetSubscriptionQueryHandler.
        var callerTenantId = tenantAccessor.MultiTenantContext?.TenantInfo?.Id
            ?? throw new UnauthorizedException("Tenant context is required.");
        var isRoot = callerTenantId == MultitenancyConstants.Root.Id;

        var invoice = await dbContext.Invoices.AsNoTracking()
            .Include(i => i.LineItems)
            .FirstOrDefaultAsync(
                i => i.Id == query.InvoiceId && (isRoot || i.TenantId == callerTenantId),
                cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Invoice {query.InvoiceId} not found.");

        return invoice.ToDto();
    }
}

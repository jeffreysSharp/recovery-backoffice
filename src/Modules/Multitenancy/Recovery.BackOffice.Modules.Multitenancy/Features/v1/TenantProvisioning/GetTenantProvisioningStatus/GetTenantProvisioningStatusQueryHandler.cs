using Recovery.BackOffice.Modules.Multitenancy.Contracts.Dtos;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.TenantProvisioning;
using Recovery.BackOffice.Modules.Multitenancy.Provisioning;
using Mediator;

namespace Recovery.BackOffice.Modules.Multitenancy.Features.v1.TenantProvisioning.GetTenantProvisioningStatus;

public sealed class GetTenantProvisioningStatusQueryHandler(ITenantProvisioningService provisioningService)
    : IQueryHandler<GetTenantProvisioningStatusQuery, TenantProvisioningStatusDto>
{
    public async ValueTask<TenantProvisioningStatusDto> Handle(GetTenantProvisioningStatusQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        return await provisioningService.GetStatusAsync(query.TenantId, cancellationToken).ConfigureAwait(false);
    }
}
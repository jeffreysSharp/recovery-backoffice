using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Modules.Identity.Contracts.Services;
using Recovery.BackOffice.Modules.Identity.Contracts.v1.Impersonation;
using Recovery.BackOffice.Modules.Identity.Contracts.v1.Impersonation.GetImpersonationGrants;
using Mediator;

namespace Recovery.BackOffice.Modules.Identity.Features.v1.Impersonation.GetImpersonationGrants;

public sealed class GetImpersonationGrantsQueryHandler(
    IImpersonationGrantService grantService,
    ICurrentUser currentUser)
    : IQueryHandler<GetImpersonationGrantsQuery, IReadOnlyList<ImpersonationGrantDto>>
{
    public async ValueTask<IReadOnlyList<ImpersonationGrantDto>> Handle(
        GetImpersonationGrantsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var callerTenant = currentUser.GetTenant()
            ?? throw new UnauthorizedException("missing tenant context");
        var isRoot = string.Equals(callerTenant, MultitenancyConstants.Root.Id, StringComparison.Ordinal);

        // Tenant scoping: root operators target any tenant; tenant admins are locked to their
        // own regardless of input. Mirrors the StartImpersonation cross-tenant rule.
        var tenantFilter = isRoot ? request.ImpersonatedTenantId : callerTenant;

        return await grantService.ListAsync(
            status: request.Status,
            impersonatedTenantId: tenantFilter,
            actorUserId: request.ActorUserId,
            take: request.Take,
            ct: cancellationToken).ConfigureAwait(false);
    }
}

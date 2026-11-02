using Finbuckle.MultiTenant.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Authorization;
using Recovery.BackOffice.Modules.Multitenancy.Contracts;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.UpdateTenantTheme;
using Mediator;

namespace Recovery.BackOffice.Modules.Multitenancy.Features.v1.UpdateTenantTheme;

public sealed class UpdateTenantThemeCommandHandler(
    ITenantThemeService themeService,
    IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    : ICommandHandler<UpdateTenantThemeCommand>
{
    public async ValueTask<Unit> Handle(UpdateTenantThemeCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tenantId = tenantAccessor.MultiTenantContext?.TenantInfo?.Id
            ?? throw new InvalidOperationException("No agency context available");

        await themeService.UpdateThemeAsync(tenantId, command.Theme, cancellationToken);

        return Unit.Value;
    }
}
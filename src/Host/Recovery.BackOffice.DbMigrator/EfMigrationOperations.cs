using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;

namespace Recovery.BackOffice.DbMigrator;

public sealed class EfMigrationOperations(MigrationDbContextFactory factory, TimeProvider clock) : IMigrationOperations
{
    public async Task MigrateCatalogAsync(CancellationToken cancellationToken)
    {
        await using var context = factory.CreateCatalog();
        await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task EnsureRootTenantAsync(CancellationToken cancellationToken)
    {
        await using var context = factory.CreateCatalog();
        var root = await context.TenantInfo.FindAsync([MultitenancyConstants.Root.Id], cancellationToken).ConfigureAwait(false);
        if (root is not null) return;

        context.TenantInfo.Add(new AppTenantInfo(
            MultitenancyConstants.Root.Id, MultitenancyConstants.Root.Id, MultitenancyConstants.Root.Name)
        {
            AdminEmail = MultitenancyConstants.Root.EmailAddress,
            Issuer = MultitenancyConstants.Root.Issuer,
            IsActive = true,
            ValidUpto = clock.GetUtcNow().UtcDateTime.AddYears(1)
        });
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<AppTenantInfo>> GetTenantsAsync(CancellationToken cancellationToken)
    {
        await using var context = factory.CreateCatalog();
        return await context.TenantInfo.AsNoTracking().OrderBy(tenant => tenant.Id)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task MigrateIdentityAsync(AppTenantInfo tenant, CancellationToken cancellationToken)
    {
        await using var context = factory.CreateIdentity(tenant);
        await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    }
}

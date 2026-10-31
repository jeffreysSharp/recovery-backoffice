using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;

namespace Recovery.BackOffice.DbMigrator;

public interface IMigrationOperations
{
    Task MigrateCatalogAsync(CancellationToken cancellationToken);
    Task EnsureRootTenantAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<AppTenantInfo>> GetTenantsAsync(CancellationToken cancellationToken);
    Task MigrateIdentityAsync(AppTenantInfo tenant, CancellationToken cancellationToken);
}

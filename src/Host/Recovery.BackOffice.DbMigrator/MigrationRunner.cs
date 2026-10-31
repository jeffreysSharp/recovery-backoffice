using Microsoft.Extensions.Logging;

namespace Recovery.BackOffice.DbMigrator;

public sealed class MigrationRunner(IMigrationOperations operations, ILogger<MigrationRunner> logger)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        logger.LogInformation("Applying tenant catalog migrations.");
        await operations.MigrateCatalogAsync(cancellationToken).ConfigureAwait(false);
        await operations.EnsureRootTenantAsync(cancellationToken).ConfigureAwait(false);
        var tenants = await operations.GetTenantsAsync(cancellationToken).ConfigureAwait(false);
        if (tenants.Count == 0)
        {
            throw new InvalidOperationException("No tenants were found after initializing the root tenant.");
        }
        foreach (var tenant in tenants)
        {
            cancellationToken.ThrowIfCancellationRequested();
            logger.LogInformation("Applying Identity migrations for tenant {TenantId}.", tenant.Id);
            await operations.MigrateIdentityAsync(tenant, cancellationToken).ConfigureAwait(false);
        }
        logger.LogInformation("Database migrations completed for {TenantCount} tenants.", tenants.Count);
    }
}

using Finbuckle.MultiTenant.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Persistence.Context;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Recovery.BackOffice.Modules.Auditing.Persistence;

public sealed class AuditDbContext : BaseDbContext
{
    public AuditDbContext(
    IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
    DbContextOptions<AuditDbContext> options,
    IOptions<DatabaseOptions> settings,
    IHostEnvironment environment) : base(multiTenantContextAccessor, options, settings, environment) { }

    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditDbContext).Assembly);
        modelBuilder.HasDbFunction(typeof(AuditJsonFunctions).GetMethod(nameof(AuditJsonFunctions.Value))!)
            .HasName("JSON_VALUE").IsBuiltIn();

        // base.OnModelCreating runs LAST so BaseDbContext's auto-apply sees
        // fully-configured entities (including HasMany child types).
        base.OnModelCreating(modelBuilder);
    }
}

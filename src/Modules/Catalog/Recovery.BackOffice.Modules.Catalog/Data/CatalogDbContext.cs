using Finbuckle.MultiTenant.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Persistence.Context;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Recovery.BackOffice.Modules.Catalog.Data;

public sealed class CatalogDbContext : BaseDbContext
{
    public const string Schema = "catalog";

    public CatalogDbContext(
        IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
        DbContextOptions<CatalogDbContext> options,
        IOptions<DatabaseOptions> settings,
        IHostEnvironment environment) : base(multiTenantContextAccessor, options, settings, environment) { }

    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly,
            type => type != typeof(Configurations.CountryConfiguration) && type != typeof(Configurations.StateConfiguration) && type != typeof(Configurations.CityConfiguration) && type != typeof(Configurations.AgencyTypeConfiguration));
        // base.OnModelCreating runs LAST so BaseDbContext's auto-apply sees
        // fully-configured entities (including HasMany child types like ProductImage).
        base.OnModelCreating(modelBuilder);
    }
}

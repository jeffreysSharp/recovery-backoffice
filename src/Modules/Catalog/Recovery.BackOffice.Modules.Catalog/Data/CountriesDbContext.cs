using Recovery.BackOffice.Modules.Catalog.Data.Configurations;
using Recovery.BackOffice.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Catalog.Data;

/// <summary>Uses the central connection, like BillingDbContext, without per-agency connection switching.</summary>
public sealed class CountriesDbContext(DbContextOptions<CountriesDbContext> options) : DbContext(options)
{
    public DbSet<Country> Countries => Set<Country>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(CatalogDbContext.Schema);
        modelBuilder.ApplyConfiguration(new CountryConfiguration());
    }
}

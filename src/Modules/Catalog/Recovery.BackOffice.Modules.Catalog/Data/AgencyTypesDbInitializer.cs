using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.Modules.Catalog.Domain;
namespace Recovery.BackOffice.Modules.Catalog.Data;
public sealed class AgencyTypesDbInitializer(CountriesDbContext db, ILogger<AgencyTypesDbInitializer> logger) : IDbInitializer
{
    // The existing CountriesDbInitializer migrates the shared central context.
    public Task MigrateAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var existing = await db.AgencyTypes.AsNoTracking().Select(agencyType => agencyType.Name)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        var names = existing.Select(AgencyType.NormalizeName).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = AgencyTypeSeedData.BuildAgencyTypes().Where(agencyType => names.Add(AgencyType.NormalizeName(agencyType.Name))).ToList();
        if (missing.Count == 0) return;
        db.AgencyTypes.AddRange(missing);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("[AgencyTypes] seeded {Count} missing global agency types", missing.Count);
    }
}

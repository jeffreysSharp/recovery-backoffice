using Recovery.BackOffice.BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Recovery.BackOffice.Modules.Catalog.Data;

public sealed class CountriesDbInitializer(CountriesDbContext dbContext, ILogger<CountriesDbInitializer> logger) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[Countries] applied central catalog migrations");
        }
    }
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var existing = await dbContext.Countries.AsNoTracking()
            .Select(country => new { country.Iso2Code, country.Iso3Code })
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        var iso2Codes = existing.Select(country => country.Iso2Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var iso3Codes = existing.Select(country => country.Iso3Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        int inserted = 0;

        foreach (var country in CountrySeedData.BuildCountries())
        {
            // Existing rows (including inactive or administratively edited rows) are never synchronized.
            if (iso2Codes.Contains(country.Iso2Code)) continue;
            if (iso3Codes.Contains(country.Iso3Code))
            {
                logger.LogWarning("[Countries] skipped ISO 2 {Iso2Code}: ISO 3 {Iso3Code} is already assigned; existing data preserved",
                    country.Iso2Code, country.Iso3Code);
                continue;
            }

            dbContext.Countries.Add(country);
            inserted++;
        }

        if (inserted == 0) return;
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("[Countries] seeded {Count} missing global countries", inserted);
        }
    }
}

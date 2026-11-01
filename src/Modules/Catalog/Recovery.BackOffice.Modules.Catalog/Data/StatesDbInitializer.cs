using Recovery.BackOffice.BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Recovery.BackOffice.Modules.Catalog.Data;

public sealed class StatesDbInitializer(CountriesDbContext dbContext, ILogger<StatesDbInitializer> logger) : IDbInitializer
{
    // The shared central context is migrated by CountriesDbInitializer, registered immediately before this initializer.
    public Task MigrateAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var brazilId = await dbContext.Countries.AsNoTracking().Where(country => country.Iso2Code == "BR")
            .Select(country => (Guid?)country.Id).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("País BR não encontrado. Execute a carga de Countries antes da carga de States.");
        var existing = await dbContext.States.AsNoTracking().Where(state => state.CountryId == brazilId)
            .Select(state => new { state.Code, state.IbgeCode }).ToListAsync(cancellationToken).ConfigureAwait(false);
        var codes = existing.Select(state => state.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var ibgeCodes = existing.Where(state => state.IbgeCode != null).Select(state => state.IbgeCode!).ToHashSet(StringComparer.Ordinal);
        int inserted = 0;
        foreach (var state in StateSeedData.BuildBrazilStates(brazilId))
        {
            if (codes.Contains(state.Code)) continue;
            if (ibgeCodes.Contains(state.IbgeCode!))
            {
                logger.LogWarning("[States] skipped UF {Code}: IBGE {IbgeCode} is already assigned; existing data preserved", state.Code, state.IbgeCode);
                continue;
            }
            dbContext.States.Add(state);
            inserted++;
        }
        if (inserted == 0) return;
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("[States] seeded {Count} missing Brazilian states", inserted);
    }
}

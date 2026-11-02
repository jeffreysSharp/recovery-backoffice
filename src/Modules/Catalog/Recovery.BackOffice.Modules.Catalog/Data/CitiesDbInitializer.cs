using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.Modules.Catalog.Domain;
namespace Recovery.BackOffice.Modules.Catalog.Data;
public sealed class CitiesDbInitializer(CountriesDbContext db, ILogger<CitiesDbInitializer> logger) : IDbInitializer
{
    // CountriesDbInitializer migrates this shared context before States and Cities are seeded.
    public Task MigrateAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var records = CitySeedData.BuildRecords();
        if (records.Count != CitySeedData.ExpectedCount || records.Select(row => row.IbgeCode).Distinct(StringComparer.Ordinal).Count() != records.Count ||
            records.Select(row => row.StateIbgeCode).Distinct(StringComparer.Ordinal).Count() != 27 ||
            records.Any(row => row.IbgeCode.Length != 7 || row.IbgeCode.Any(character => character is < '0' or > '9') ||
                row.StateIbgeCode.Length != 2 || !row.IbgeCode.StartsWith(row.StateIbgeCode, StringComparison.Ordinal) ||
                string.IsNullOrWhiteSpace(row.Name) || row.Name.Length is < 2 or > 150))
            throw new InvalidOperationException("Base local de municípios incompleta ou inválida; nenhuma cidade foi inserida.");
        var states = await db.States.AsNoTracking().Join(db.Countries, state => state.CountryId, country => country.Id,
                (state, country) => new { state.Id, state.IbgeCode, country.Iso2Code })
            .Where(row => row.Iso2Code == "BR" && row.IbgeCode != null).ToListAsync(cancellationToken).ConfigureAwait(false);
        var stateIds = states.ToDictionary(row => row.IbgeCode!, row => row.Id, StringComparer.Ordinal);
        var missing = records.Select(row => row.StateIbgeCode).Distinct(StringComparer.Ordinal).Where(code => !stateIds.ContainsKey(code)).ToArray();
        if (missing.Length > 0)
            throw new InvalidOperationException("UFs brasileiras não encontradas: " + string.Join(", ", missing) + ". Execute Countries e States antes de Cities.");
        var existing = await db.Cities.AsNoTracking().Select(city => new { city.IbgeCode, city.StateId, city.Name })
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        var codes = existing.Where(city => city.IbgeCode != null).Select(city => city.IbgeCode!).ToHashSet(StringComparer.Ordinal);
        var names = existing.Select(city => (city.StateId, NameKey(city.Name))).ToHashSet();
        var pending = new List<City>();
        foreach (var record in records)
        {
            if (codes.Contains(record.IbgeCode)) continue;
            var stateId = stateIds[record.StateIbgeCode];
            if (!names.Add((stateId, NameKey(record.Name))))
            {
                logger.LogWarning("[Cities] skipped IBGE {IbgeCode}: name already exists in UF {StateIbgeCode}; administrative data preserved", record.IbgeCode, record.StateIbgeCode);
                continue;
            }
            pending.Add(City.Create(stateId, record.Name, record.IbgeCode));
            codes.Add(record.IbgeCode);
        }
        if (pending.Count == 0) return;
        db.Cities.AddRange(pending);
        // SaveChanges uses the provider's SQL batching and one transaction for the entire load.
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("[Cities] seeded {Count} missing cities from IBGE reference {ReferenceDate}", pending.Count, CitySeedData.ReferenceDate);
    }
    private static string NameKey(string name) => Convert.ToHexString(CultureInfo.InvariantCulture.CompareInfo
        .GetSortKey(City.NormalizeName(name), CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace).KeyData);
}

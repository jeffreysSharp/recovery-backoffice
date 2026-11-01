using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;
using Testcontainers.MsSql;

namespace Recovery.BackOffice.Integration.Tests.Tests.Catalog;

/// <summary>Real SQL Server seeding through the existing initializer; no web host or HTTP context.</summary>
public sealed class CountrySeedTests : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Integration_Test_Only123!")
        .WithAutoRemove(true)
        .WithCleanUp(true)
        .Build();

    public async Task InitializeAsync()
    {
        await _sqlServer.StartAsync();
        await using var connection = new SqlConnection(_sqlServer.GetConnectionString());
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "CREATE DATABASE [CountrySeedTests]";
        await command.ExecuteNonQueryAsync();
        await using var db = CreateContext();
        await Initializer(db).MigrateAsync(CancellationToken.None);
    }

    public Task DisposeAsync() => _sqlServer.DisposeAsync().AsTask();

    private CountriesDbContext CreateContext()
    {
        var connection = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "CountrySeedTests"
        }.ConnectionString;
        var options = new DbContextOptionsBuilder<CountriesDbContext>().UseSqlServer(connection,
            sql => sql.MigrationsAssembly("Recovery.BackOffice.Migrations.SqlServer")).Options;
        return new CountriesDbContext(options);
    }

    private static CountriesDbInitializer Initializer(CountriesDbContext db) =>
        new(db, NullLogger<CountriesDbInitializer>.Instance);

    [Fact]
    public async Task EmptyDatabase_Receives249GlobalCountries_WithoutHttpServices()
    {
        await using var db = CreateContext();
        (await db.Countries.CountAsync()).ShouldBe(0);
        await Initializer(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        var countries = await db.Countries.AsNoTracking().ToListAsync();
        countries.Count.ShouldBe(249);
        countries.Select(country => country.Iso2Code).Distinct(StringComparer.Ordinal).Count().ShouldBe(249);
        countries.Select(country => country.Iso3Code).Distinct(StringComparer.Ordinal).Count().ShouldBe(249);
        var brazil = countries.Single(country => country.Iso2Code == "BR");
        brazil.Name.ShouldBe("Brasil");
        brazil.Iso3Code.ShouldBe("BRA");
        brazil.NumericCode.ShouldBe("076");
        brazil.PhoneCode.ShouldBe("+55");
        countries.Single(country => country.Iso2Code == "ST").Name.ShouldBe("São Tomé e Príncipe");
        countries.Count(country => country.PhoneCode is null).ShouldBe(7);
        db.Model.FindEntityType(typeof(Country))!.FindProperty("TenantId").ShouldBeNull();
    }

    [Fact]
    public async Task RepeatedExecution_PreservesAllIdsAndAdministrativeEdits()
    {
        await using var db = CreateContext();
        await Initializer(db).SeedAsync(CancellationToken.None);
        var ids = await db.Countries.AsNoTracking().ToDictionaryAsync(country => country.Iso2Code, country => country.Id);
        var brazil = await db.Countries.SingleAsync(country => country.Iso2Code == "BR");
        brazil.Update("Brasil administrativo", "BR", "ZZZ", "999", "+99", false);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        await Initializer(db).SeedAsync(CancellationToken.None);
        await Initializer(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        var after = await db.Countries.AsNoTracking().ToListAsync();
        after.Count.ShouldBe(249);
        foreach (var country in after) country.Id.ShouldBe(ids[country.Iso2Code]);
        brazil = after.Single(country => country.Iso2Code == "BR");
        brazil.Name.ShouldBe("Brasil administrativo");
        brazil.Iso3Code.ShouldBe("ZZZ");
        brazil.NumericCode.ShouldBe("999");
        brazil.PhoneCode.ShouldBe("+99");
        brazil.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task PartialCatalog_InsertsOnlyMissingCountries_AndPreservesCustomRecords()
    {
        await using var db = CreateContext();
        var brazil = Country.Create("Brasil personalizado", "BR", "BRA", "999", null, false);
        var custom = Country.Create("Registro administrativo", "ZZ", "ZZZ", null, "+99", false);
        db.Countries.AddRange(brazil, custom);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        await Initializer(db).SeedAsync(CancellationToken.None);
        await Initializer(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        (await db.Countries.CountAsync()).ShouldBe(250);
        var existing = await db.Countries.SingleAsync(country => country.Iso2Code == "BR");
        existing.Id.ShouldBe(brazil.Id);
        existing.Name.ShouldBe("Brasil personalizado");
        existing.NumericCode.ShouldBe("999");
        existing.PhoneCode.ShouldBeNull();
        existing.IsActive.ShouldBeFalse();
        (await db.Countries.SingleAsync(country => country.Iso2Code == "ZZ")).Id.ShouldBe(custom.Id);
    }

    [Fact]
    public async Task ConflictingIso3_DoesNotOverwriteAdministrativeDataOrViolateConstraints()
    {
        await using var db = CreateContext();
        var custom = Country.Create("Código ajustado administrativamente", "ZZ", "BRA");
        db.Countries.Add(custom);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        await Initializer(db).SeedAsync(CancellationToken.None);
        await Initializer(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        (await db.Countries.CountAsync()).ShouldBe(249);
        (await db.Countries.AnyAsync(country => country.Iso2Code == "BR")).ShouldBeFalse();
        var preserved = await db.Countries.SingleAsync(country => country.Iso2Code == "ZZ");
        preserved.Id.ShouldBe(custom.Id);
        preserved.Name.ShouldBe("Código ajustado administrativamente");
        preserved.Iso3Code.ShouldBe("BRA");
    }
}

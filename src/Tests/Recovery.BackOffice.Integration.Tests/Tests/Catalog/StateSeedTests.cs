using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;
using Testcontainers.MsSql;

namespace Recovery.BackOffice.Integration.Tests.Tests.Catalog;

/// <summary>Real SQL Server seeding through the existing initializer; no web host or HTTP context.</summary>
public sealed class StateSeedTests : IAsyncLifetime
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
        command.CommandText = "CREATE DATABASE [StateSeedTests]";
        await command.ExecuteNonQueryAsync();
        await using var db = CreateContext();
        await Initializer(db).MigrateAsync(CancellationToken.None);
    }

    public Task DisposeAsync() => _sqlServer.DisposeAsync().AsTask();

    private CountriesDbContext CreateContext()
    {
        var connection = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "StateSeedTests"
        }.ConnectionString;
        var options = new DbContextOptionsBuilder<CountriesDbContext>().UseSqlServer(connection,
            sql => sql.MigrationsAssembly("Recovery.BackOffice.Migrations.SqlServer")).Options;
        return new CountriesDbContext(options);
    }

    private static CountriesDbInitializer Initializer(CountriesDbContext db) =>
        new(db, NullLogger<CountriesDbInitializer>.Instance);


    private static StatesDbInitializer StatesInitializer(CountriesDbContext db) =>
        new(db, NullLogger<StatesDbInitializer>.Instance);

    [Fact]
    public async Task EmptyStates_Receives27BrazilianUfs_AfterCountriesSeed()
    {
        await using var db = CreateContext();
        await Initializer(db).SeedAsync(CancellationToken.None);
        (await db.States.CountAsync()).ShouldBe(0);
        await StatesInitializer(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        var states = await db.States.AsNoTracking().ToListAsync();
        var brazilId = await db.Countries.Where(country => country.Iso2Code == "BR").Select(country => country.Id).SingleAsync();
        states.Count.ShouldBe(27);
        states.All(state => state.CountryId == brazilId && state.IsActive).ShouldBeTrue();
        states.Single(state => state.Code == "SP").IbgeCode.ShouldBe("35");
        states.Single(state => state.Code == "DF").IbgeCode.ShouldBe("53");
        states.Single(state => state.Code == "SP").Name.ShouldBe("São Paulo");
        db.Model.FindEntityType(typeof(State))!.FindProperty("TenantId").ShouldBeNull();
    }

    [Fact]
    public async Task RepeatedExecution_PreservesIds_AdministrativeEdits_AndCustomStates()
    {
        await using var db = CreateContext();
        await Initializer(db).SeedAsync(CancellationToken.None);
        await StatesInitializer(db).SeedAsync(CancellationToken.None);
        var ids = await db.States.AsNoTracking().ToDictionaryAsync(state => state.Code, state => state.Id);
        var sp = await db.States.SingleAsync(state => state.Code == "SP");
        sp.Update(sp.CountryId, "São Paulo personalizado", "SP", "99", false);
        var custom = State.Create(sp.CountryId, "Estado personalizado", "CUSTOM", "98", false);
        db.States.Add(custom);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        await StatesInitializer(db).SeedAsync(CancellationToken.None);
        await StatesInitializer(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        var after = await db.States.AsNoTracking().ToListAsync();
        after.Count.ShouldBe(28);
        foreach (var state in after.Where(state => state.Code != "CUSTOM")) state.Id.ShouldBe(ids[state.Code]);
        sp = after.Single(state => state.Code == "SP");
        sp.Name.ShouldBe("São Paulo personalizado");
        sp.IbgeCode.ShouldBe("99");
        sp.IsActive.ShouldBeFalse();
        after.Single(state => state.Code == "CUSTOM").Id.ShouldBe(custom.Id);
    }

    [Fact]
    public async Task MissingBrazil_FailsClearly_WithoutCreatingCountries()
    {
        await using var db = CreateContext();
        var error = await Should.ThrowAsync<InvalidOperationException>(() => StatesInitializer(db).SeedAsync(CancellationToken.None));
        error.Message.ShouldContain("BR");
        (await db.Countries.CountAsync()).ShouldBe(0);
        (await db.States.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task PartialCatalog_UsesExistingBrazilId_AndPreservesForeignStates()
    {
        await using var db = CreateContext();
        var brazil = Country.Create("Brasil administrativo", "BR", "BRA", "076", null, false);
        var portugal = Country.Create("Portugal", "PT", "PRT");
        db.Countries.AddRange(brazil, portugal);
        await db.SaveChangesAsync();
        var sp = State.Create(brazil.Id, "SP administrativo", "SP", "99", false);
        var foreign = State.Create(portugal.Id, "Subdivisão estrangeira", "LONG-CODE");
        db.States.AddRange(sp, foreign);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        await StatesInitializer(db).SeedAsync(CancellationToken.None);
        await StatesInitializer(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        (await db.States.CountAsync(state => state.CountryId == brazil.Id)).ShouldBe(27);
        (await db.States.CountAsync(state => state.CountryId == portugal.Id)).ShouldBe(1);
        (await db.States.SingleAsync(state => state.Id == sp.Id)).Name.ShouldBe("SP administrativo");
        (await db.States.SingleAsync(state => state.Id == foreign.Id)).IbgeCode.ShouldBeNull();
        (await db.Countries.CountAsync()).ShouldBe(2);
    }

    [Fact]
    public async Task ConflictingIbge_IsNotOverwrittenOrDuplicated()
    {
        await using var db = CreateContext();
        var brazil = Country.Create("Brasil", "BR", "BRA");
        db.Countries.Add(brazil);
        await db.SaveChangesAsync();
        var existing = State.Create(brazil.Id, "Administrativo", "CUSTOM", "35", false);
        db.States.Add(existing);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        await StatesInitializer(db).SeedAsync(CancellationToken.None);
        await StatesInitializer(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        (await db.States.CountAsync()).ShouldBe(27);
        (await db.States.AnyAsync(state => state.Code == "SP")).ShouldBeFalse();
        (await db.States.SingleAsync(state => state.IbgeCode == "35")).Id.ShouldBe(existing.Id);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Database_EnforcesCompositeUniqueness(bool duplicateCode)
    {
        await using var db = CreateContext();
        var brazil = Country.Create("Brasil", "BR", "BRA");
        db.Countries.Add(brazil);
        await db.SaveChangesAsync();
        db.States.Add(State.Create(brazil.Id, "São Paulo", "SP", "35"));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        db.States.Add(State.Create(brazil.Id, "Duplicado", duplicateCode ? "SP" : "CUSTOM", duplicateCode ? "99" : "35"));
        await Should.ThrowAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task ForeignKey_RejectsUnknownCountry_AndBlocksCountryDeletion()
    {
        await using var db = CreateContext();
        db.States.Add(State.Create(Guid.NewGuid(), "Sem país", "XX"));
        await Should.ThrowAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();
        await Initializer(db).SeedAsync(CancellationToken.None);
        await StatesInitializer(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        var brazil = await db.Countries.SingleAsync(country => country.Iso2Code == "BR");
        db.Countries.Remove(brazil);
        await Should.ThrowAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }
}


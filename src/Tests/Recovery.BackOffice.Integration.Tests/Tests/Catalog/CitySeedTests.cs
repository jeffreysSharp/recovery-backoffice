using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;
using Testcontainers.MsSql;

namespace Recovery.BackOffice.Integration.Tests.Tests.Catalog;

/// <summary>Real SQL Server seeding through the existing initializer; no web host or HTTP context.</summary>
public sealed class CitySeedTests : IAsyncLifetime
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
        command.CommandText = "CREATE DATABASE [CitySeedTests]";
        await command.ExecuteNonQueryAsync();
        await using var db = CreateContext();
        await Initializer(db).MigrateAsync(CancellationToken.None);
    }

    public Task DisposeAsync() => _sqlServer.DisposeAsync().AsTask();

    private CountriesDbContext CreateContext()
    {
        var connection = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "CitySeedTests"
        }.ConnectionString;
        var options = new DbContextOptionsBuilder<CountriesDbContext>().UseSqlServer(connection,
            sql => sql.MigrationsAssembly("Recovery.BackOffice.Migrations.SqlServer")).Options;
        return new CountriesDbContext(options);
    }

    private static CountriesDbInitializer Initializer(CountriesDbContext db) =>
        new(db, NullLogger<CountriesDbInitializer>.Instance);


    private static CitiesDbInitializer Cities(CountriesDbContext db) => new(db, NullLogger<CitiesDbInitializer>.Instance);
    private static async Task PrepareStatesAsync(CountriesDbContext db)
    {
        await Initializer(db).SeedAsync(CancellationToken.None);
        await new StatesDbInitializer(db, NullLogger<StatesDbInitializer>.Instance).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
    }
    [Fact]
    public async Task EmptyCities_ReceivesCompleteOfficialSnapshot_WithCorrectDynamicStateIds()
    {
        await using var db = CreateContext();
        await PrepareStatesAsync(db);
        (await db.Cities.CountAsync()).ShouldBe(0);
        await Cities(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        (await db.Cities.CountAsync()).ShouldBe(CitySeedData.ExpectedCount);
        var rows = await db.Cities.Join(db.States, city => city.StateId, state => state.Id, (city,state) => new { city.IbgeCode, city.IsActive, StateCode = state.IbgeCode })
            .ToListAsync();
        rows.All(row => row.IsActive && row.IbgeCode!.StartsWith(row.StateCode!, StringComparison.Ordinal)).ShouldBeTrue();
        foreach (var (code,name,uf) in new[] { ("3550308","São Paulo","35"), ("3513801","Diadema","35"), ("5300108","Brasília","53") })
        {
            var city = await db.Cities.SingleAsync(city => city.IbgeCode == code);
            city.Name.ShouldBe(name);
            city.StateId.ShouldBe(await db.States.Where(state => state.IbgeCode == uf).Select(state => state.Id).SingleAsync());
        }
        var metadata = db.Model.FindEntityType(typeof(City))!;
        metadata.FindProperty("TenantId").ShouldBeNull();
        metadata.FindProperty("CountryId").ShouldBeNull();
    }
    [Fact]
    public async Task RepeatedSeeding_PreservesIds_AdministrativeEdits_AndCustomCities()
    {
        await using var db = CreateContext();
        await PrepareStatesAsync(db);
        await Cities(db).SeedAsync(CancellationToken.None);
        var ids = await db.Cities.AsNoTracking().ToDictionaryAsync(city => city.IbgeCode!, city => city.Id);
        var sp = await db.Cities.SingleAsync(city => city.IbgeCode == "3550308");
        sp.Update(sp.StateId, "São Paulo administrativo", sp.IbgeCode, false);
        var custom = City.Create(sp.StateId, "Cidade administrativa", "0000001", false);
        db.Cities.Add(custom);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        await Cities(db).SeedAsync(CancellationToken.None);
        await Cities(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        var after = await db.Cities.AsNoTracking().ToListAsync();
        after.Count.ShouldBe(CitySeedData.ExpectedCount + 1);
        foreach (var city in after.Where(city => city.Id != custom.Id)) city.Id.ShouldBe(ids[city.IbgeCode!]);
        sp = after.Single(city => city.IbgeCode == "3550308");
        sp.Name.ShouldBe("São Paulo administrativo");
        sp.IsActive.ShouldBeFalse();
        after.Single(city => city.Id == custom.Id).IsActive.ShouldBeFalse();
    }
    [Fact]
    public async Task MissingUf_StopsEntireLoad_BeforeInsertingAnyCity()
    {
        await using var db = CreateContext();
        await PrepareStatesAsync(db);
        var df = await db.States.SingleAsync(state => state.IbgeCode == "53");
        db.States.Remove(df);
        await db.SaveChangesAsync();
        var error = await Should.ThrowAsync<InvalidOperationException>(() => Cities(db).SeedAsync(CancellationToken.None));
        error.Message.ShouldContain("53");
        (await db.Cities.CountAsync()).ShouldBe(0);
    }
    [Fact]
    public async Task MissingCountriesAndStates_DoesNotCreateInventedParents()
    {
        await using var db = CreateContext();
        await Should.ThrowAsync<InvalidOperationException>(() => Cities(db).SeedAsync(CancellationToken.None));
        (await db.Countries.CountAsync()).ShouldBe(0);
        (await db.States.CountAsync()).ShouldBe(0);
        (await db.Cities.CountAsync()).ShouldBe(0);
    }
    [Fact]
    public async Task ConflictingNormalizedName_IsLoggedAndPreserved_WithoutPartialSourceData()
    {
        await using var db = CreateContext();
        await PrepareStatesAsync(db);
        var sp = await db.States.SingleAsync(state => state.IbgeCode == "35");
        var existing = City.Create(sp.Id, "SAO PAULO", "0000002", false);
        db.Cities.Add(existing);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        await Cities(db).SeedAsync(CancellationToken.None);
        await Cities(db).SeedAsync(CancellationToken.None);
        (await db.Cities.CountAsync()).ShouldBe(CitySeedData.ExpectedCount);
        (await db.Cities.AnyAsync(city => city.IbgeCode == "3550308")).ShouldBeFalse();
        (await db.Cities.SingleAsync(city => city.Id == existing.Id)).Name.ShouldBe("SAO PAULO");
    }
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Database_RejectsDuplicateIbgeOrNormalizedName(bool duplicateIbge)
    {
        await using var db = CreateContext();
        await PrepareStatesAsync(db);
        var sp = await db.States.SingleAsync(state => state.IbgeCode == "35");
        db.Cities.Add(City.Create(sp.Id, "São Paulo", "3550308"));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        db.Cities.Add(City.Create(sp.Id, duplicateIbge ? "Outro município" : "  sAO pAULO ", duplicateIbge ? "3550308" : "0000003"));
        await Should.ThrowAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }
    [Fact]
    public async Task Database_RequiresExistingState_AndRestrictsParentDeletion()
    {
        await using var db = CreateContext();
        db.Cities.Add(City.Create(Guid.NewGuid(), "Sem estado", "0000001"));
        await Should.ThrowAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();
        await PrepareStatesAsync(db);
        var sp = await db.States.SingleAsync(state => state.IbgeCode == "35");
        db.Cities.Add(City.Create(sp.Id, "São Paulo", "3550308"));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        sp = await db.States.SingleAsync(state => state.IbgeCode == "35");
        db.States.Remove(sp);
        await Should.ThrowAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }
}


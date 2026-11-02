using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;
using Testcontainers.MsSql;

namespace Recovery.BackOffice.Integration.Tests.Tests.Catalog;

/// <summary>Real SQL Server seeding through the existing initializer; no web host or HTTP context.</summary>
public sealed class AgencyTypeSeedTests : IAsyncLifetime
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
        command.CommandText = "CREATE DATABASE [AgencyTypeSeedTests]";
        await command.ExecuteNonQueryAsync();
        await using var db = CreateContext();
        await Initializer(db).MigrateAsync(CancellationToken.None);
    }

    public Task DisposeAsync() => _sqlServer.DisposeAsync().AsTask();

    private CountriesDbContext CreateContext()
    {
        var connection = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "AgencyTypeSeedTests"
        }.ConnectionString;
        var options = new DbContextOptionsBuilder<CountriesDbContext>().UseSqlServer(connection,
            sql => sql.MigrationsAssembly("Recovery.BackOffice.Migrations.SqlServer")).Options;
        return new CountriesDbContext(options);
    }

    private static CountriesDbInitializer Initializer(CountriesDbContext db) =>
        new(db, NullLogger<CountriesDbInitializer>.Instance);


    private static AgencyTypesDbInitializer AgencyTypes(CountriesDbContext db) => new(db, NullLogger<AgencyTypesDbInitializer>.Instance);
    [Fact]
    public async Task EmptyCatalog_ReceivesExactlyFourActiveTypes_WithoutParentOrHttpDependencies()
    {
        await using var db = CreateContext();
        (await db.AgencyTypes.CountAsync()).ShouldBe(0);
        await AgencyTypes(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        var rows = await db.AgencyTypes.AsNoTracking().ToListAsync();
        rows.Count.ShouldBe(4);
        rows.All(row=>row.IsActive).ShouldBeTrue();
        var expected = AgencyTypeSeedData.BuildAgencyTypes().ToDictionary(row=>row.Name,row=>row.Description,StringComparer.Ordinal);
        foreach(var row in rows) row.Description.ShouldBe(expected[row.Name]);
        (await db.Countries.CountAsync()).ShouldBe(0);
        (await db.States.CountAsync()).ShouldBe(0);
        (await db.Cities.CountAsync()).ShouldBe(0);
        db.Model.FindEntityType(typeof(AgencyType))!.FindProperty("TenantId").ShouldBeNull();
    }
    [Fact]
    public async Task RepeatedSeeding_PreservesIds_AdministrativeDescriptions_InactiveStatus_AndCustomTypes()
    {
        await using var db = CreateContext();
        await AgencyTypes(db).SeedAsync(CancellationToken.None);
        var ids = await db.AgencyTypes.AsNoTracking().ToDictionaryAsync(row=>row.Name,row=>row.Id,StringComparer.OrdinalIgnoreCase);
        var existing = await db.AgencyTypes.SingleAsync(row=>row.Name=="Escritório de Cobrança");
        existing.Update("  ESCRITÓRIO\t de  Cobrança ","Descrição administrativa",false);
        var custom = AgencyType.Create("Tipo administrativo","Personalizado",false);
        db.AgencyTypes.Add(custom);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        await AgencyTypes(db).SeedAsync(CancellationToken.None);
        await AgencyTypes(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        var after = await db.AgencyTypes.AsNoTracking().ToListAsync();
        after.Count.ShouldBe(5);
        foreach(var row in after.Where(row=>row.Id!=custom.Id)) row.Id.ShouldBe(ids[row.Name]);
        existing=after.Single(row=>row.Id==existing.Id);
        existing.Name.ShouldBe("ESCRITÓRIO de Cobrança");
        existing.Description.ShouldBe("Descrição administrativa");
        existing.IsActive.ShouldBeFalse();
        after.Single(row=>row.Id==custom.Id).IsActive.ShouldBeFalse();
    }
    [Fact]
    public async Task PartialCatalog_MatchesExistingNamesWithExcessSpaces_WithoutRewritingThem()
    {
        await using var db=CreateContext();
        var existing=AgencyType.Create("Escritório Jurídico","Descrição preservada",false);
        db.AgencyTypes.Add(existing);
        await db.SaveChangesAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE [catalog].[AgencyTypes] SET [Name] = {"  ESCRITÓRIO   JURÍDICO  "} WHERE [Id] = {existing.Id}");
        db.ChangeTracker.Clear();
        await AgencyTypes(db).SeedAsync(CancellationToken.None);
        await AgencyTypes(db).SeedAsync(CancellationToken.None);
        db.ChangeTracker.Clear();
        (await db.AgencyTypes.CountAsync()).ShouldBe(4);
        var after=await db.AgencyTypes.SingleAsync(row=>row.Id==existing.Id);
        after.Name.ShouldBe("  ESCRITÓRIO   JURÍDICO  ");
        after.Description.ShouldBe("Descrição preservada");
        after.IsActive.ShouldBeFalse();
    }
    [Fact]
    public async Task DatabaseUniqueIndex_RejectsConcurrentNormalizedDuplicates()
    {
        await using var db=CreateContext();
        var existing=AgencyType.Create("Escritório Jurídico");
        db.AgencyTypes.Add(existing);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        db.AgencyTypes.Add(AgencyType.Create("  ESCRITÓRIO\t JURÍDICO  "));
        await Should.ThrowAsync<DbUpdateException>(()=>db.SaveChangesAsync());
    }
}


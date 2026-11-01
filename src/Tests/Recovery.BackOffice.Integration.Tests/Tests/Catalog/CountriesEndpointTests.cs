#pragma warning disable CA1308 // Lowercase input deliberately verifies ISO normalization and case-insensitive search.
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Integration.Tests.Infrastructure;
using Recovery.BackOffice.Integration.Tests.Infrastructure.Extensions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Integration.Tests.Tests.Catalog;

[Collection(RecoveryCollectionDefinition.Name)]
public sealed class CountriesEndpointTests(RecoveryWebApplicationFactory factory)
{
    private const string Base = "/api/v1/catalog/countries";
    private static int _sequence;
    private readonly AuthHelper _auth = new(factory);
    private static object Payload(string iso2, string iso3, string name = "País de teste", bool active = true) =>
        new { name, iso2Code = iso2, iso3Code = iso3, numericCode = "004", phoneCode = "+55", isActive = active };
    private static (string Iso2, string Iso3) Codes()
    {
        var letter = (char)('A' + Interlocked.Increment(ref _sequence));
        return ("Q" + letter, "QQ" + letter);
    }
    private static async Task<Guid> CreateAsync(HttpClient client)
    {
        var codes = Codes();
        var response = await client.PostAsJsonAsync(Base, Payload(codes.Iso2.ToLowerInvariant(), codes.Iso3.ToLowerInvariant()));
        response.StatusCode.ShouldBe(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        return await response.DeserializeAsync<Guid>();
    }

    [Fact]
    public async Task Crud_Normalizes_PreservesNumericCode_AndSetsAuditMetadata()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var id = await CreateAsync(client);
        var response = await client.GetAsync(Base + "/" + id);
        var country = await response.DeserializeAsync<CountryDto>();
        country.NumericCode.ShouldBe("004");
        country.Iso2Code.ShouldBe(country.Iso2Code.ToUpperInvariant());
        using (var scope = factory.Services.CreateScope())
        {
            var entity = await scope.ServiceProvider.GetRequiredService<CountriesDbContext>().Countries.SingleAsync(x => x.Id == id);
            entity.CreatedOnUtc.ShouldNotBe(default);
            entity.CreatedBy.ShouldNotBeNullOrWhiteSpace();
        }
        var update = await client.PutAsJsonAsync(Base + "/" + id, Payload(country.Iso2Code,country.Iso3Code,"Atualizado",false));
        update.StatusCode.ShouldBe(HttpStatusCode.OK);
        country = await (await client.GetAsync(Base + "/" + id)).DeserializeAsync<CountryDto>();
        country.Name.ShouldBe("Atualizado");
        country.IsActive.ShouldBeFalse();
        (await client.PutAsJsonAsync(Base + "/" + id,Payload(country.Iso2Code,country.Iso3Code,"Atualizado",true))).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await client.DeleteAsync(Base + "/" + id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await client.GetAsync(Base + "/" + id)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("", "BR", "BRA", null, null)]
    [InlineData(" ", "BR", "BRA", null, null)]
    [InlineData("Brasil", "B1", "BRA", null, null)]
    [InlineData("Brasil", "BR", "", null, null)]
    [InlineData("Brasil", "BR", "BRA", "04", null)]
    [InlineData("Brasil", "BR", "BRA", null, "55")]
    public async Task Create_RejectsInvalidFields(string name,string iso2,string iso3,string? numeric,string? phone)
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var response = await client.PostAsJsonAsync(Base,new { name,iso2Code=iso2,iso3Code=iso3,numericCode=numeric,phoneCode=phone });
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DuplicateCodes_AreRejectedByValidationAndDatabase()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var id = await CreateAsync(client);
        var country = await (await client.GetAsync(Base + "/" + id)).DeserializeAsync<CountryDto>();
        var other = Codes();
        (await client.PostAsJsonAsync(Base,Payload(country.Iso2Code.ToLowerInvariant(),other.Iso3))).IsSuccessStatusCode.ShouldBeFalse();
        (await client.PostAsJsonAsync(Base,Payload(other.Iso2,country.Iso3Code.ToLowerInvariant()))).IsSuccessStatusCode.ShouldBeFalse();
        foreach (var duplicateIso2 in new[] { true, false })
        {
            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CountriesDbContext>();
            var codes = Codes();
            db.Countries.Add(Country.Create("Duplicado",duplicateIso2 ? country.Iso2Code : codes.Iso2,duplicateIso2 ? codes.Iso3 : country.Iso3Code));
            await Should.ThrowAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
    }

    [Fact]
    public async Task Search_FiltersStatus_SearchesIso_AndPaginates()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var id = await CreateAsync(client);
        var country = await (await client.GetAsync(Base + "/" + id)).DeserializeAsync<CountryDto>();
        await client.PutAsJsonAsync(Base + "/" + id,Payload(country.Iso2Code,country.Iso3Code,"País ordenado",false));
        var response = await client.GetAsync(Base+"?search="+country.Iso2Code.ToLowerInvariant()+"&isActive=false&pageSize=1&pageNumber=1&sortBy=iso2Code&sortDir=desc");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var page = await response.DeserializeAsync<PagedResponse<CountryDto>>();
        page.Items.ShouldContain(x => x.Id == id);
        page.PageSize.ShouldBe(1);
        page.Items.Count.ShouldBeLessThanOrEqualTo(1);
        (await client.GetAsync(Base+"?pageNumber=0")).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_RejectsForeignKeyReferences()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var id = await CreateAsync(client);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CountriesDbContext>();
        // A test-only consumer models a future referencing table, without adding another domain entity.
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE [catalog].[CountryReferenceTest] ([Id] int PRIMARY KEY, [CountryId] uniqueidentifier NOT NULL REFERENCES [catalog].[Countries]([Id]) ON DELETE NO ACTION)");
        try
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO [catalog].[CountryReferenceTest] VALUES (1, {id})");
            (await client.DeleteAsync(Base + "/" + id)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
            (await client.GetAsync(Base + "/" + id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        }
        finally { await db.Database.ExecuteSqlRawAsync("DROP TABLE [catalog].[CountryReferenceTest]"); }
        (await client.DeleteAsync(Base + "/" + id)).StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CentralContext_DoesNotSwitchToAgencyDatabase()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var id = await CreateAsync(client);
        using var scope = factory.Services.CreateScope();
        var setter = scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
        setter.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(new AppTenantInfo("other","Other",
            "Server=127.0.0.1,1;Database=unreachable;User Id=sa;Password=NotReal!123;Connect Timeout=1;TrustServerCertificate=true",
            "other@example.test","test"));
        var db = scope.ServiceProvider.GetRequiredService<CountriesDbContext>();
        (await db.Countries.AnyAsync(x => x.Id == id)).ShouldBeTrue();
        db.Model.FindEntityType(typeof(Country))!.FindProperty("TenantId").ShouldBeNull();
        using var catalogScope = factory.Services.CreateScope();
        catalogScope.ServiceProvider.GetRequiredService<CatalogDbContext>().Model.FindEntityType(typeof(Country)).ShouldBeNull();
    }

    [Fact]
    public async Task Agencies_ReadSharedCatalog_ButCannotMutateIt()
    {
        using var root = await _auth.CreateRootAdminClientAsync();
        var id = await CreateAsync(root);
        var tenantId = "country-" + Guid.NewGuid().ToString("N")[..8];
        var email = tenantId+"@example.test";
        var response = await root.PostAsJsonAsync(TestConstants.TenantsBasePath,new { id=tenantId,name=tenantId,adminEmail=email,adminPassword=TestConstants.DefaultPassword,issuer="test.issuer" });
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        await TenantProvisioningWait.WaitForProvisioningAsync(root,tenantId);
        using var agency = await _auth.CreateAuthenticatedClientAsync(email,TestConstants.DefaultPassword,tenantId);
        (await agency.GetAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        var codes = Codes();
        (await agency.PostAsJsonAsync(Base,Payload(codes.Iso2,codes.Iso3))).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await agency.PutAsJsonAsync(Base+"/"+id,Payload(codes.Iso2,codes.Iso3))).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await agency.DeleteAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Anonymous_CannotRead()
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("tenant","root");
        (await client.GetAsync(Base)).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_CanStartInactive_AndUpdateCannotDuplicateAnotherCountry()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var firstId = await CreateAsync(client);
        var first = await (await client.GetAsync(Base + "/" + firstId)).DeserializeAsync<CountryDto>();
        var codes = Codes();
        var created = await client.PostAsJsonAsync(Base, Payload(codes.Iso2, codes.Iso3, "Inativo", false));
        created.StatusCode.ShouldBe(HttpStatusCode.OK);
        var secondId = await created.DeserializeAsync<Guid>();
        var second = await (await client.GetAsync(Base + "/" + secondId)).DeserializeAsync<CountryDto>();
        second.IsActive.ShouldBeFalse();
        var duplicate = await client.PutAsJsonAsync(Base + "/" + secondId, Payload(first.Iso2Code, codes.Iso3));
        duplicate.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        second = await (await client.GetAsync(Base + "/" + secondId)).DeserializeAsync<CountryDto>();
        second.Iso2Code.ShouldBe(codes.Iso2);
    }

    [Fact]
    public async Task Search_OrdersResultsAndReturnsDifferentPages()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var prefix = "Ordem " + Guid.NewGuid().ToString("N");
        foreach (var suffix in new[] { " A", " B" })
        {
            var codes = Codes();
            (await client.PostAsJsonAsync(Base, Payload(codes.Iso2, codes.Iso3, prefix + suffix))).StatusCode.ShouldBe(HttpStatusCode.OK);
        }
        var query = Base + "?search=" + Uri.EscapeDataString(prefix) + "&pageSize=1&sortBy=name&sortDir=desc&pageNumber=";
        var first = await (await client.GetAsync(query + "1")).DeserializeAsync<PagedResponse<CountryDto>>();
        var second = await (await client.GetAsync(query + "2")).DeserializeAsync<PagedResponse<CountryDto>>();
        first.TotalCount.ShouldBe(2);
        first.TotalPages.ShouldBe(2);
        first.Items.Single().Name.ShouldBe(prefix + " B");
        second.Items.Single().Name.ShouldBe(prefix + " A");
    }
}

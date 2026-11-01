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
public sealed class StatesEndpointTests(RecoveryWebApplicationFactory factory)
{
    private const string Base = "/api/v1/catalog/states";
    private readonly AuthHelper _auth = new(factory);
    private async Task<Guid> CountryIdAsync(string iso2)
    {
        using var scope = factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<CountriesDbContext>().Countries
            .Where(country => country.Iso2Code == iso2).Select(country => country.Id).SingleAsync();
    }
    private static object Payload(Guid countryId, string code, string name = "Estado de teste", string? ibgeCode = null, bool active = true) =>
        new { countryId, name, code, ibgeCode, isActive = active };
    private static string Code() => Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    private static async Task<Guid> CreateAsync(HttpClient client, Guid countryId, string? code = null, string name = "Estado de teste")
    {
        var response = await client.PostAsJsonAsync(Base, Payload(countryId, code ?? Code(), name));
        response.StatusCode.ShouldBe(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        return await response.DeserializeAsync<Guid>();
    }

    [Fact]
    public async Task Crud_Normalizes_CountryJoin_Audit_Activation_AndDeletion()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var countryId = await CountryIdAsync("PT");
        var code = Code();
        var id = await CreateAsync(client, countryId, " " + code + " ", "  Estado criado  ");
        var dto = await (await client.GetAsync(Base + "/" + id)).DeserializeAsync<StateDto>();
        dto.Code.ShouldBe(code);
        dto.CountryName.ShouldBe("Portugal");
        dto.Name.ShouldBe("Estado criado");
        dto.IbgeCode.ShouldBeNull();
        using (var scope = factory.Services.CreateScope())
        {
            var entity = await scope.ServiceProvider.GetRequiredService<CountriesDbContext>().States.SingleAsync(state => state.Id == id);
            entity.CreatedOnUtc.ShouldNotBe(default);
            entity.CreatedBy.ShouldNotBeNullOrWhiteSpace();
        }
        (await client.PutAsJsonAsync(Base + "/" + id, Payload(countryId, code, "Atualizado", active: false))).StatusCode.ShouldBe(HttpStatusCode.OK);
        dto = await (await client.GetAsync(Base + "/" + id)).DeserializeAsync<StateDto>();
        dto.Name.ShouldBe("Atualizado");
        dto.IsActive.ShouldBeFalse();
        (await client.PutAsJsonAsync(Base + "/" + id, Payload(countryId, code, active: true))).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await client.DeleteAsync(Base + "/" + id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await client.GetAsync(Base + "/" + id)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Codes_AreUniquePerCountry_AndIbgeIsBrazilOnly()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var brazil = await CountryIdAsync("BR");
        var portugal = await CountryIdAsync("PT");
        var code = Code();
        var id = await CreateAsync(client, brazil, code);
        await CreateAsync(client, portugal, code);
        (await client.PostAsJsonAsync(Base, Payload(brazil, code))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.PostAsJsonAsync(Base, Payload(brazil, Code(), ibgeCode: "35"))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.PostAsJsonAsync(Base, Payload(portugal, Code(), ibgeCode: "35"))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.PutAsJsonAsync(Base + "/" + id, Payload(portugal, code, ibgeCode: "99"))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var dto = await (await client.GetAsync(Base + "/" + id)).DeserializeAsync<StateDto>();
        dto.CountryId.ShouldBe(brazil);
    }

    [Theory]
    [InlineData("A", "XX", null)]
    [InlineData("Válido", "", null)]
    [InlineData("Válido", "ABCDEFGHIJK", null)]
    [InlineData("Válido", "XX", "1")]
    [InlineData("Válido", "XX", "123")]
    [InlineData("Válido", "XX", "AB")]
    public async Task Create_RejectsInvalidFields(string name, string code, string? ibgeCode)
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var response = await client.PostAsJsonAsync(Base, Payload(await CountryIdAsync("BR"), code, name, ibgeCode));
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UnknownCountry_AndInvalidPagination_AreRejected()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        foreach (var countryId in new[] { Guid.Empty, Guid.NewGuid() })
            (await client.PostAsJsonAsync(Base, Payload(countryId, Code()))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.GetAsync(Base + "?pageNumber=0")).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.GetAsync(Base + "?pageSize=201")).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_FiltersCountryAndStatus_OrdersAndPaginates()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var brazil = await CountryIdAsync("BR");
        var portugal = await CountryIdAsync("PT");
        var prefix = "Ordem " + Guid.NewGuid().ToString("N");
        var a = await CreateAsync(client, portugal, name: prefix + " A");
        var b = await CreateAsync(client, portugal, name: prefix + " B");
        await CreateAsync(client, brazil, name: prefix + " C");
        foreach (var id in new[] { a, b })
        {
            var dto = await (await client.GetAsync(Base + "/" + id)).DeserializeAsync<StateDto>();
            (await client.PutAsJsonAsync(Base + "/" + id, Payload(portugal, dto.Code, dto.Name, active: false))).StatusCode.ShouldBe(HttpStatusCode.OK);
        }
        var query = Base + "?search=" + Uri.EscapeDataString(prefix) + "&countryId=" + portugal +
            "&isActive=false&pageSize=1&sortBy=name&sortDir=desc&pageNumber=";
        var first = await (await client.GetAsync(query + "1")).DeserializeAsync<PagedResponse<StateDto>>();
        var second = await (await client.GetAsync(query + "2")).DeserializeAsync<PagedResponse<StateDto>>();
        first.TotalCount.ShouldBe(2);
        first.TotalPages.ShouldBe(2);
        first.Items.Single().Id.ShouldBe(b);
        second.Items.Single().Id.ShouldBe(a);
    }

    [Fact]
    public async Task RestrictiveForeignKeys_BlockCountryAndDependentStateDeletion()
    {
        using var client = await _auth.CreateRootAdminClientAsync();
        var brazil = await CountryIdAsync("BR");
        (await client.DeleteAsync("/api/v1/catalog/countries/" + brazil)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var id = await CreateAsync(client, brazil);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CountriesDbContext>();
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE [catalog].[StateReferenceTest] ([Id] int PRIMARY KEY, [StateId] uniqueidentifier NOT NULL REFERENCES [catalog].[States]([Id]) ON DELETE NO ACTION)");
        try
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO [catalog].[StateReferenceTest] VALUES (1, {id})");
            (await client.DeleteAsync(Base + "/" + id)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
        }
        finally { await db.Database.ExecuteSqlRawAsync("DROP TABLE [catalog].[StateReferenceTest]"); }
        (await client.DeleteAsync(Base + "/" + id)).StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GlobalContext_DoesNotSwitchToAgencyDatabase_OrAddTenantIsolation()
    {
        using var root = await _auth.CreateRootAdminClientAsync();
        var id = await CreateAsync(root, await CountryIdAsync("BR"));
        using var scope = factory.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>().MultiTenantContext =
            new MultiTenantContext<AppTenantInfo>(new AppTenantInfo("other", "Other",
                "Server=127.0.0.1,1;Database=unreachable;User Id=sa;Password=NotReal!123;Connect Timeout=1;TrustServerCertificate=true",
                "other@example.test", "test"));
        var db = scope.ServiceProvider.GetRequiredService<CountriesDbContext>();
        (await db.States.AnyAsync(state => state.Id == id)).ShouldBeTrue();
        db.Model.FindEntityType(typeof(State))!.FindProperty("TenantId").ShouldBeNull();
        using var catalogScope = factory.Services.CreateScope();
        catalogScope.ServiceProvider.GetRequiredService<CatalogDbContext>().Model.FindEntityType(typeof(State)).ShouldBeNull();
    }

    [Fact]
    public async Task Agencies_ReadSharedStates_ButCannotMaintainThem()
    {
        using var root = await _auth.CreateRootAdminClientAsync();
        var id = await CreateAsync(root, await CountryIdAsync("BR"));
        var tenantId = "state-" + Guid.NewGuid().ToString("N")[..8];
        var email = tenantId + "@example.test";
        (await root.PostAsJsonAsync(TestConstants.TenantsBasePath, new { id = tenantId, name = tenantId,
            adminEmail = email, adminPassword = TestConstants.DefaultPassword, issuer = "test.issuer" })).StatusCode.ShouldBe(HttpStatusCode.Created);
        await TenantProvisioningWait.WaitForProvisioningAsync(root, tenantId);
        using var agency = await _auth.CreateAuthenticatedClientAsync(email, TestConstants.DefaultPassword, tenantId);
        (await agency.GetAsync(Base + "/" + id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        var payload = Payload(await CountryIdAsync("BR"), Code());
        (await agency.PostAsJsonAsync(Base, payload)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await agency.PutAsJsonAsync(Base + "/" + id, payload)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await agency.DeleteAsync(Base + "/" + id)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Anonymous_CannotRead()
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("tenant", "root");
        (await client.GetAsync(Base)).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

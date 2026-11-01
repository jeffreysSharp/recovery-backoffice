using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using System.Globalization;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Integration.Tests.Infrastructure;
using Recovery.BackOffice.Integration.Tests.Infrastructure.Extensions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Catalog.Domain;
namespace Recovery.BackOffice.Integration.Tests.Tests.Catalog;

[Collection(RecoveryCollectionDefinition.Name)]
public sealed class CitiesEndpointTests(RecoveryWebApplicationFactory factory)
{
    private const string Base = "/api/v1/catalog/cities";
    private readonly AuthHelper _auth = new(factory);
    private static int _sequence;
    private static string Ibge() => "9" + Interlocked.Increment(ref _sequence).ToString("D6", CultureInfo.InvariantCulture);
    private static object Payload(Guid stateId, string name, string? ibgeCode, bool active = true) => new { stateId,name,ibgeCode,isActive=active };
    private async Task<Guid> BrazilStateAsync(string uf = "35")
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CountriesDbContext>();
        return await db.States.Join(db.Countries, state=>state.CountryId,country=>country.Id,(state,country)=>new{state.Id,state.IbgeCode,country.Iso2Code})
            .Where(row=>row.Iso2Code=="BR" && row.IbgeCode==uf).Select(row=>row.Id).SingleAsync();
    }
    private async Task<Guid> ForeignStateAsync(HttpClient client)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CountriesDbContext>();
        var portugal = await db.Countries.Where(country=>country.Iso2Code=="PT").Select(country=>country.Id).SingleAsync();
        var response=await client.PostAsJsonAsync("/api/v1/catalog/states",new{countryId=portugal,name="Estado estrangeiro",code=Guid.NewGuid().ToString("N")[..8]});
        response.StatusCode.ShouldBe(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        return await response.DeserializeAsync<Guid>();
    }
    private static async Task<Guid> CreateAsync(HttpClient client, Guid stateId, string? name = null, string? ibgeCode = null)
    {
        var response=await client.PostAsJsonAsync(Base,Payload(stateId,name ?? "Cidade "+Guid.NewGuid().ToString("N"),ibgeCode ?? Ibge()));
        response.StatusCode.ShouldBe(HttpStatusCode.OK,await response.Content.ReadAsStringAsync());
        return await response.DeserializeAsync<Guid>();
    }
    [Fact]
    public async Task Crud_ProjectsStateAndCountry_SetsAudit_UpdatesAndDeletes()
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        var stateId=await BrazilStateAsync();
        var code=Ibge();
        var id=await CreateAsync(client,stateId,"  Cidade criada  ",code);
        var dto=await (await client.GetAsync(Base+"/"+id)).DeserializeAsync<CityDto>();
        dto.Name.ShouldBe("Cidade criada");
        dto.StateName.ShouldBe("São Paulo");
        dto.CountryName.ShouldBe("Brasil");
        dto.IbgeCode.ShouldBe(code);
        using(var scope=factory.Services.CreateScope())
        {
            var entity=await scope.ServiceProvider.GetRequiredService<CountriesDbContext>().Cities.SingleAsync(city=>city.Id==id);
            entity.CreatedOnUtc.ShouldNotBe(default);
            entity.CreatedBy.ShouldNotBeNullOrWhiteSpace();
        }
        (await client.PutAsJsonAsync(Base+"/"+id,Payload(stateId,"Atualizada",code,false))).StatusCode.ShouldBe(HttpStatusCode.OK);
        dto=await (await client.GetAsync(Base+"/"+id)).DeserializeAsync<CityDto>();
        dto.Name.ShouldBe("Atualizada");
        dto.IsActive.ShouldBeFalse();
        (await client.PutAsJsonAsync(Base+"/"+id,Payload(await BrazilStateAsync("53"),"Atualizada",code,true))).StatusCode.ShouldBe(HttpStatusCode.OK);
        dto=await (await client.GetAsync(Base+"/"+id)).DeserializeAsync<CityDto>();
        dto.StateName.ShouldBe("Distrito Federal");
        dto.IsActive.ShouldBeTrue();
        (await client.DeleteAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await client.GetAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    [Theory]
    [InlineData("", "1234567")]
    [InlineData("A", "1234567")]
    [InlineData("Cidade", null)]
    [InlineData("Cidade", "123456")]
    [InlineData("Cidade", "12345678")]
    [InlineData("Cidade", "12345AB")]
    public async Task BrazilianCities_ValidateNameAndRequiredSevenDigitIbge(string name,string? code)
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        (await client.PostAsJsonAsync(Base,Payload(await BrazilStateAsync(),name,code))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task StateAndNameValidation_RejectsUnknownIdsAndLongNames()
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        foreach(var id in new[]{Guid.Empty,Guid.NewGuid()})
            (await client.PostAsJsonAsync(Base,Payload(id,"Cidade",Ibge()))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.PostAsJsonAsync(Base,Payload(await BrazilStateAsync(),new string('x',151),Ibge()))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task ForeignCities_AllowNullIbge_ButMovingToBrazilRequiresIt()
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        var stateId=await ForeignStateAsync(client);
        var response=await client.PostAsJsonAsync(Base,Payload(stateId,"Cidade estrangeira",null));
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var id=await response.DeserializeAsync<Guid>();
        var dto=await (await client.GetAsync(Base+"/"+id)).DeserializeAsync<CityDto>();
        dto.IbgeCode.ShouldBeNull();
        (await client.PutAsJsonAsync(Base+"/"+id,Payload(await BrazilStateAsync(),dto.Name,null))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await (await client.GetAsync(Base+"/"+id)).DeserializeAsync<CityDto>()).StateId.ShouldBe(stateId);
    }
    [Fact]
    public async Task Duplicates_RespectGlobalIbgeAndNormalizedNamePerState()
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        var stateId=await BrazilStateAsync();
        var suffix=Guid.NewGuid().ToString("N");
        var name="São Teste "+suffix;
        var code=Ibge();
        await CreateAsync(client,stateId,name,code);
        (await client.PostAsJsonAsync(Base,Payload(stateId,"Outro nome "+suffix,code))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.PostAsJsonAsync(Base,Payload(stateId,"  SAO TESTE "+suffix+"  ",Ibge()))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await CreateAsync(client,await BrazilStateAsync("53"),name);
        var second=await CreateAsync(client,stateId,"Segunda "+suffix);
        (await client.PutAsJsonAsync(Base+"/"+second,Payload(stateId,name,Ibge()))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task Search_FiltersCountryStateAndStatus_AndPaginates()
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        var sp=await BrazilStateAsync();
        var prefix="Filtro "+Guid.NewGuid().ToString("N");
        var a=await CreateAsync(client,sp,prefix+" A");
        var b=await CreateAsync(client,sp,prefix+" B");
        await CreateAsync(client,await BrazilStateAsync("53"),prefix+" C");
        await CreateAsync(client,await ForeignStateAsync(client),prefix+" D");
        var dto=await (await client.GetAsync(Base+"/"+a)).DeserializeAsync<CityDto>();
        (await client.PutAsJsonAsync(Base+"/"+a,Payload(sp,dto.Name,dto.IbgeCode,false))).StatusCode.ShouldBe(HttpStatusCode.OK);
        var query=Base+"?search="+Uri.EscapeDataString(prefix)+"&countryId="+dto.CountryId+"&pageSize=1&sortBy=name&sortDir=desc&pageNumber=";
        var first=await (await client.GetAsync(query+"1")).DeserializeAsync<PagedResponse<CityDto>>();
        var second=await (await client.GetAsync(query+"2")).DeserializeAsync<PagedResponse<CityDto>>();
        first.TotalCount.ShouldBe(3);
        first.Items.Single().Name.ShouldBe(prefix+" C");
        second.Items.Single().Id.ShouldBe(b);
        var filtered=await (await client.GetAsync(Base+"?stateId="+sp+"&isActive=false&search="+Uri.EscapeDataString(prefix))).DeserializeAsync<PagedResponse<CityDto>>();
        filtered.Items.Single().Id.ShouldBe(a);
        var byCode=await (await client.GetAsync(Base+"?search="+dto.IbgeCode)).DeserializeAsync<PagedResponse<CityDto>>();
        byCode.Items.Single().Id.ShouldBe(a);
        (await client.GetAsync(Base+"?pageNumber=0")).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.GetAsync(Base+"?pageSize=201")).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task ForeignKeys_ProtectStateAndReferencedCityDeletion()
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        var stateId=await BrazilStateAsync();
        (await client.DeleteAsync("/api/v1/catalog/states/"+stateId)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var id=await CreateAsync(client,stateId);
        using var scope=factory.Services.CreateScope();
        var db=scope.ServiceProvider.GetRequiredService<CountriesDbContext>();
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE [catalog].[CityReferenceTest] ([Id] int PRIMARY KEY, [CityId] uniqueidentifier NOT NULL REFERENCES [catalog].[Cities]([Id]) ON DELETE NO ACTION)");
        try
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO [catalog].[CityReferenceTest] VALUES (1, {id})");
            (await client.DeleteAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
        }
        finally { await db.Database.ExecuteSqlRawAsync("DROP TABLE [catalog].[CityReferenceTest]"); }
        (await client.DeleteAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.OK);
    }
    [Fact]
    public async Task GlobalCatalog_DoesNotSwitchToAgencyDatabase()
    {
        using var root=await _auth.CreateRootAdminClientAsync();
        var id=await CreateAsync(root,await BrazilStateAsync());
        using var scope=factory.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>().MultiTenantContext=new MultiTenantContext<AppTenantInfo>(
            new AppTenantInfo("other","Other","Server=127.0.0.1,1;Database=unreachable;User Id=sa;Password=NotReal!123;Connect Timeout=1;TrustServerCertificate=true","other@example.test","test"));
        var db=scope.ServiceProvider.GetRequiredService<CountriesDbContext>();
        (await db.Cities.AnyAsync(city=>city.Id==id)).ShouldBeTrue();
        db.Model.FindEntityType(typeof(City))!.FindProperty("TenantId").ShouldBeNull();
        db.Model.FindEntityType(typeof(City))!.FindProperty("CountryId").ShouldBeNull();
        using var catalogScope=factory.Services.CreateScope();
        catalogScope.ServiceProvider.GetRequiredService<CatalogDbContext>().Model.FindEntityType(typeof(City)).ShouldBeNull();
    }
    [Fact]
    public async Task Agencies_CanReadSharedCity_ButCannotMutate()
    {
        using var root=await _auth.CreateRootAdminClientAsync();
        var stateId=await BrazilStateAsync();
        var id=await CreateAsync(root,stateId);
        var tenantId="city-"+Guid.NewGuid().ToString("N")[..8];
        var email=tenantId+"@example.test";
        (await root.PostAsJsonAsync(TestConstants.TenantsBasePath,new{id=tenantId,name=tenantId,adminEmail=email,adminPassword=TestConstants.DefaultPassword,issuer="test.issuer"}))
            .StatusCode.ShouldBe(HttpStatusCode.Created);
        await TenantProvisioningWait.WaitForProvisioningAsync(root,tenantId);
        using var agency=await _auth.CreateAuthenticatedClientAsync(email,TestConstants.DefaultPassword,tenantId);
        (await agency.GetAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        var payload=Payload(stateId,"Tentativa",Ibge());
        (await agency.PostAsJsonAsync(Base,payload)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await agency.PutAsJsonAsync(Base+"/"+id,payload)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await agency.DeleteAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
    [Fact]
    public async Task Anonymous_CannotRead()
    {
        using var client=factory.CreateClient();
        client.DefaultRequestHeaders.Add("tenant","root");
        (await client.GetAsync(Base)).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

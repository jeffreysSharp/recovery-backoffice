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
public sealed class AgencyTypesEndpointTests(RecoveryWebApplicationFactory factory)
{
    private const string Base = "/api/v1/catalog/agency-types";
    private readonly AuthHelper _auth = new(factory);
    private static object Payload(string name, string? description = null, bool active = true) => new { name,description,isActive=active };
    private static async Task<Guid> CreateAsync(HttpClient client,string? name=null,string? description=null,bool active=true)
    {
        var response=await client.PostAsJsonAsync(Base,Payload(name??"Tipo "+Guid.NewGuid().ToString("N"),description,active));
        response.StatusCode.ShouldBe(HttpStatusCode.OK,await response.Content.ReadAsStringAsync());
        return await response.DeserializeAsync<Guid>();
    }
    [Fact]
    public async Task Crud_NormalizesWhitespace_SetsAudit_UpdatesAndDeletes()
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        var suffix=Guid.NewGuid().ToString("N");
        var id=await CreateAsync(client,"  Tipo\t  criado "+suffix+"  ","  Descrição inicial  ");
        var dto=await (await client.GetAsync(Base+"/"+id)).DeserializeAsync<AgencyTypeDto>();
        dto.Name.ShouldBe("Tipo criado "+suffix);
        dto.Description.ShouldBe("Descrição inicial");
        dto.IsActive.ShouldBeTrue();
        using(var scope=factory.Services.CreateScope())
        {
            var entity=await scope.ServiceProvider.GetRequiredService<CountriesDbContext>().AgencyTypes.SingleAsync(row=>row.Id==id);
            entity.CreatedOnUtc.ShouldNotBe(default);
            entity.CreatedBy.ShouldNotBeNullOrWhiteSpace();
        }
        (await client.PutAsJsonAsync(Base+"/"+id,Payload("Atualizado "+suffix,null,false))).StatusCode.ShouldBe(HttpStatusCode.OK);
        dto=await (await client.GetAsync(Base+"/"+id)).DeserializeAsync<AgencyTypeDto>();
        dto.Name.ShouldBe("Atualizado "+suffix);
        dto.Description.ShouldBeNull();
        dto.IsActive.ShouldBeFalse();
        (await client.PutAsJsonAsync(Base+"/"+id,Payload(dto.Name,"Nova descrição",true))).StatusCode.ShouldBe(HttpStatusCode.OK);
        dto=await (await client.GetAsync(Base+"/"+id)).DeserializeAsync<AgencyTypeDto>();
        dto.IsActive.ShouldBeTrue();
        dto.Description.ShouldBe("Nova descrição");
        (await client.DeleteAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await client.GetAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("AB")]
    public async Task Create_RejectsMissingOrShortNames(string name)
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        (await client.PostAsJsonAsync(Base,Payload(name))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task LengthBoundaries_ValidateOnCreateAndUpdate()
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        var name=Guid.NewGuid().ToString("N")+new string('x',68);
        var id=await CreateAsync(client,name,new string('d',500));
        (await client.PostAsJsonAsync(Base,Payload(new string('x',101)))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.PostAsJsonAsync(Base,Payload("Descrição longa",new string('d',501)))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.PutAsJsonAsync(Base+"/"+id,Payload("AB"))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.PutAsJsonAsync(Base+"/"+id,Payload(name,new string('d',501)))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var dto=await (await client.GetAsync(Base+"/"+id)).DeserializeAsync<AgencyTypeDto>();
        dto.Name.ShouldBe(name);
        dto.Description!.Length.ShouldBe(500);
    }
    [Fact]
    public async Task DuplicateNames_IgnoreCaseAndExcessWhitespace_OnCreateAndUpdate()
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        var suffix=Guid.NewGuid().ToString("N");
        var name="Escritório de Teste "+suffix;
        await CreateAsync(client,name);
        var duplicate="  ESCRITÓRIO \t de\r\n Teste "+suffix+"  ";
        (await client.PostAsJsonAsync(Base,Payload(duplicate))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var second=await CreateAsync(client);
        (await client.PutAsJsonAsync(Base+"/"+second,Payload(duplicate))).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task Search_FindsNameAndDescription_FiltersStatus_AndPaginatesByName()
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        var prefix="Ordem "+Guid.NewGuid().ToString("N");
        var description="Texto "+Guid.NewGuid().ToString("N");
        var a=await CreateAsync(client,prefix+" A",description,false);
        var b=await CreateAsync(client,prefix+" B",description,false);
        await CreateAsync(client,prefix+" C",description,true);
        var query=Base+"?search="+Uri.EscapeDataString(prefix)+"&isActive=false&pageSize=1&sortBy=name&sortDir=desc&pageNumber=";
        var first=await (await client.GetAsync(query+"1")).DeserializeAsync<PagedResponse<AgencyTypeDto>>();
        var second=await (await client.GetAsync(query+"2")).DeserializeAsync<PagedResponse<AgencyTypeDto>>();
        first.TotalCount.ShouldBe(2);
        first.TotalPages.ShouldBe(2);
        first.Items.Single().Id.ShouldBe(b);
        second.Items.Single().Id.ShouldBe(a);
        var byDescription=await (await client.GetAsync(Base+"?search="+Uri.EscapeDataString(description))).DeserializeAsync<PagedResponse<AgencyTypeDto>>();
        byDescription.TotalCount.ShouldBe(3);
        (await client.GetAsync(Base+"?pageNumber=0")).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.GetAsync(Base+"?pageSize=201")).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task Delete_RejectsDependentRecords_WithConflict()
    {
        using var client=await _auth.CreateRootAdminClientAsync();
        var id=await CreateAsync(client);
        using var scope=factory.Services.CreateScope();
        var db=scope.ServiceProvider.GetRequiredService<CountriesDbContext>();
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE [catalog].[AgencyTypeReferenceTest] ([Id] int PRIMARY KEY, [AgencyTypeId] uniqueidentifier NOT NULL REFERENCES [catalog].[AgencyTypes]([Id]) ON DELETE NO ACTION)");
        try
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO [catalog].[AgencyTypeReferenceTest] VALUES (1,{id})");
            (await client.DeleteAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
            (await client.GetAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        }
        finally {await db.Database.ExecuteSqlRawAsync("DROP TABLE [catalog].[AgencyTypeReferenceTest]");}
        (await client.DeleteAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.OK);
    }
    [Fact]
    public async Task GlobalCatalog_DoesNotSwitchToTenantDatabase_OrCreateTenantColumn()
    {
        using var root=await _auth.CreateRootAdminClientAsync();
        var id=await CreateAsync(root);
        using var scope=factory.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>().MultiTenantContext=new MultiTenantContext<AppTenantInfo>(
            new AppTenantInfo("other","Other","Server=127.0.0.1,1;Database=unreachable;User Id=sa;Password=NotReal!123;Connect Timeout=1;TrustServerCertificate=true","other@example.test","test"));
        var db=scope.ServiceProvider.GetRequiredService<CountriesDbContext>();
        (await db.AgencyTypes.AnyAsync(row=>row.Id==id)).ShouldBeTrue();
        db.Model.FindEntityType(typeof(AgencyType))!.FindProperty("TenantId").ShouldBeNull();
        using var catalogScope=factory.Services.CreateScope();
        catalogScope.ServiceProvider.GetRequiredService<CatalogDbContext>().Model.FindEntityType(typeof(AgencyType)).ShouldBeNull();
    }
    [Fact]
    public async Task Agencies_CanReadGlobalTypes_ButCannotMaintainThem()
    {
        using var root=await _auth.CreateRootAdminClientAsync();
        var id=await CreateAsync(root);
        var tenantId="type-"+Guid.NewGuid().ToString("N")[..8];
        var email=tenantId+"@example.test";
        (await root.PostAsJsonAsync(TestConstants.TenantsBasePath,new{id=tenantId,name=tenantId,adminEmail=email,adminPassword=TestConstants.DefaultPassword,issuer="test.issuer"}))
            .StatusCode.ShouldBe(HttpStatusCode.Created);
        await TenantProvisioningWait.WaitForProvisioningAsync(root,tenantId);
        using var agency=await _auth.CreateAuthenticatedClientAsync(email,TestConstants.DefaultPassword,tenantId);
        (await agency.GetAsync(Base+"/"+id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        var payload=Payload("Tentativa "+Guid.NewGuid().ToString("N"));
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

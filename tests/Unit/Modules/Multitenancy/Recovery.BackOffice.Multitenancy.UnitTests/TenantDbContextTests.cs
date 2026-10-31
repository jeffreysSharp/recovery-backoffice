using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Shared.Quota;
using Recovery.BackOffice.Multitenancy.Data;
using Xunit;

namespace Recovery.BackOffice.Multitenancy.UnitTests;

public sealed class TenantDbContextTests
{
    [Fact]
    public void Model_Should_GenerateSqlServerCatalog_When_ContextIsConfigured()
    {
        using var context = CreateContext();

        var script = context.Database.GenerateCreateScript();

        Assert.Contains("CREATE TABLE [tenant].[Tenants]", script);
        Assert.Contains("[Plan] nvarchar(64)", script);
        Assert.Contains("[QuotaLimits] nvarchar(max)", script);
        Assert.DoesNotContain("jsonb", script);
        Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
    }

    [Fact]
    public void Model_Should_KeepIdentifierUnique_When_MappingTenantCatalog()
    {
        using var context = CreateContext();
        var entity = context.Model.FindEntityType(typeof(AppTenantInfo))!;

        var index = Assert.Single(entity.GetIndexes(), candidate =>
            candidate.Properties.Select(property => property.Name).SequenceEqual(["Identifier"]));

        Assert.True(index.IsUnique);
        Assert.Equal("Id", Assert.Single(entity.FindPrimaryKey()!.Properties).Name);
    }

    [Fact]
    public void Query_Should_SearchAcrossCatalog_When_TenantHasNotBeenResolved()
    {
        using var context = CreateContext();

        var sql = context.Set<AppTenantInfo>()
            .Where(tenant => tenant.Identifier == "company-a").ToQueryString();

        Assert.Contains("[tenant].[Tenants]", sql);
        Assert.Contains("[Identifier] = N'company-a'", sql);
        Assert.DoesNotContain("TenantId", sql);
        Assert.Null(context.Model.FindEntityType(typeof(AppTenantInfo))!.FindProperty("TenantId"));
    }

    [Fact]
    public void QuotaConversion_Should_RoundTripDictionary_When_QuotasAreConfigured()
    {
        using var context = CreateContext();
        var converter = context.Model.FindEntityType(typeof(AppTenantInfo))!
            .FindProperty(nameof(AppTenantInfo.QuotaLimits))!.GetValueConverter()!;
        var quotas = new Dictionary<QuotaResource, long>
        {
            [QuotaResource.Users] = 25,
            [QuotaResource.StorageBytes] = 5_000_000_000
        };

        var json = Assert.IsType<string>(converter.ConvertToProvider(quotas));
        var restored = Assert.IsType<Dictionary<QuotaResource, long>>(converter.ConvertFromProvider(json));

        Assert.NotSame(quotas, restored);
        Assert.Equal(quotas.Count, restored.Count);
        Assert.Equal(25, restored[QuotaResource.Users]);
        Assert.Equal(5_000_000_000, restored[QuotaResource.StorageBytes]);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("null")]
    [InlineData("{}")]
    public void QuotaConversion_Should_ReturnEmptyDictionary_When_StoredJsonHasNoQuotas(string json)
    {
        using var context = CreateContext();
        var converter = context.Model.FindEntityType(typeof(AppTenantInfo))!
            .FindProperty(nameof(AppTenantInfo.QuotaLimits))!.GetValueConverter()!;

        var quotas = Assert.IsType<Dictionary<QuotaResource, long>>(converter.ConvertFromProvider(json));

        Assert.Empty(quotas);
    }

    [Fact]
    public void QuotaConversion_Should_RejectInvalidJson_When_StoredValueIsCorrupted()
    {
        using var context = CreateContext();
        var converter = context.Model.FindEntityType(typeof(AppTenantInfo))!
            .FindProperty(nameof(AppTenantInfo.QuotaLimits))!.GetValueConverter()!;

        Assert.Throws<JsonException>(() => converter.ConvertFromProvider("invalid-json"));
    }

    [Theory]
    [InlineData("add")]
    [InlineData("update")]
    [InlineData("remove")]
    public void DetectChanges_Should_TrackDictionaryMutation_When_QuotaChangesInPlace(string change)
    {
        using var context = CreateContext();
        var tenant = CreateTenant();
        tenant.QuotaLimits[QuotaResource.Users] = 10;
        context.Attach(tenant);

        switch (change)
        {
            case "add": tenant.QuotaLimits[QuotaResource.ApiCalls] = 100; break;
            case "update": tenant.QuotaLimits[QuotaResource.Users] = 20; break;
            case "remove": tenant.QuotaLimits.Remove(QuotaResource.Users); break;
        }
        context.ChangeTracker.DetectChanges();

        var entry = context.Entry(tenant).Property(entity => entity.QuotaLimits);
        Assert.True(entry.IsModified);
        Assert.NotSame(entry.CurrentValue, entry.OriginalValue);
        Assert.Equal(10, entry.OriginalValue[QuotaResource.Users]);
    }

    [Fact]
    public void DetectChanges_Should_KeepEntityUnchanged_When_QuotaValuesStayTheSame()
    {
        using var context = CreateContext();
        var tenant = CreateTenant();
        tenant.QuotaLimits[QuotaResource.Users] = 10;
        context.Attach(tenant);

        tenant.QuotaLimits[QuotaResource.Users] = 10;
        context.ChangeTracker.DetectChanges();

        Assert.Equal(EntityState.Unchanged, context.Entry(tenant).State);
    }

    private static AppTenantInfo CreateTenant() => new("company-a", "company-a", "Company A")
    {
        AdminEmail = "admin@example.test"
    };

    private static TenantDbContext CreateContext() => new(
        new DbContextOptionsBuilder<TenantDbContext>()
            .UseSqlServer("Server=localhost;Database=TenantCatalogTests;Integrated Security=True")
            .Options);
}

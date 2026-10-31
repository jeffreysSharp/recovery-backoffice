using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Domain;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Persistence.UnitTests;

public sealed class TenantIsolationTests
{
    [Fact]
    public void ApplyTenantIsolationByDefault_Should_FilterByCurrentTenant_When_ModelsAreShared()
    {
        // Arrange
        using var first = CreateContext("company-a");
        using var second = CreateContext("company-b");

        // Act
        var firstSql = first.Set<Customer>().ToQueryString();
        var secondSql = second.Set<Customer>().ToQueryString();

        // Assert: cached model must still use the current context's tenant.
        Assert.Same(first.Model, second.Model);
        Assert.Contains("WHERE", firstSql);
        Assert.Contains("[TenantId] =", firstSql);
        Assert.Contains("company-a", firstSql);
        Assert.DoesNotContain("company-b", firstSql);
        Assert.Contains("[TenantId] =", secondSql);
        Assert.Contains("company-b", secondSql);
        Assert.DoesNotContain("company-a", secondSql);
    }

    [Fact]
    public void ApplyTenantIsolationByDefault_Should_LeaveGlobalEntitiesUnfiltered_When_MarkerIsPresent()
    {
        using var context = CreateContext("company-a");

        var sql = context.Set<Country>().ToQueryString();

        Assert.DoesNotContain("TenantId", sql);
        Assert.DoesNotContain("WHERE", sql);
        Assert.Null(context.Model.FindEntityType(typeof(Country))!.FindProperty("TenantId"));
    }

    [Fact]
    public void ApplyTenantIsolationByDefault_Should_ScopeUniqueIndexes_When_EntityIsTenantScoped()
    {
        using var context = CreateContext("company-a");

        var index = Assert.Single(context.Model.FindEntityType(typeof(Customer))!.GetIndexes());

        Assert.True(index.IsUnique);
        Assert.Equal(new[] { "Code", "TenantId" }, index.Properties.Select(property => property.Name));
    }

    [Fact]
    public void ApplyTenantIsolationByDefault_Should_KeepGlobalUniqueIndexes_When_MarkerIsPresent()
    {
        using var context = CreateContext("company-a");

        var index = Assert.Single(context.Model.FindEntityType(typeof(Country))!.GetIndexes());

        Assert.True(index.IsUnique);
        Assert.Equal("Code", Assert.Single(index.Properties).Name);
    }

    [Fact]
    public void ApplyTenantIsolationByDefault_Should_SkipOwnedAndKeylessTypes_When_ModelContainsBoth()
    {
        using var context = CreateContext("company-a");

        var owned = context.Model.FindEntityType(typeof(Address))!;
        var keyless = context.Model.FindEntityType(typeof(ReportRow))!;

        Assert.True(owned.IsOwned());
        Assert.Null(owned.FindProperty("TenantId"));
        Assert.Null(keyless.FindPrimaryKey());
        Assert.Null(keyless.FindProperty("TenantId"));
    }

    [Fact]
    public void ApplyTenantIsolationByDefault_Should_PreserveExplicitIsolation_When_AlreadyConfigured()
    {
        using var context = CreateContext("company-a");

        var sql = context.Set<ExplicitEntity>().ToQueryString();

        Assert.Contains("[TenantId] =", sql);
        Assert.Contains("company-a", sql);
        Assert.Single(context.Model.FindEntityType(typeof(ExplicitEntity))!.GetProperties(),
            property => property.Name == "TenantId");
    }

    [Fact]
    public void ApplyTenantIsolationByDefault_Should_RejectNull_When_ModelBuilderIsMissing()
    {
        Assert.Throws<ArgumentNullException>(() =>
            TenantIsolationExtensions.ApplyTenantIsolationByDefault(null!));
    }

    private static IsolationContext CreateContext(string tenantId)
    {
        var accessor = new AsyncLocalMultiTenantContextAccessor<AppTenantInfo>();
        ((IMultiTenantContextSetter)accessor).MultiTenantContext =
            new MultiTenantContext<AppTenantInfo>(new AppTenantInfo(tenantId, tenantId));
        var options = new DbContextOptionsBuilder<IsolationContext>()
            .UseSqlServer("Server=localhost;Database=IsolationTests;Integrated Security=True")
            .Options;
        return new IsolationContext(accessor, options);
    }

    private sealed class IsolationContext(
        IMultiTenantContextAccessor accessor,
        DbContextOptions<IsolationContext> options) : MultiTenantDbContext(accessor, options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasKey(entity => entity.Id);
            modelBuilder.Entity<Customer>().HasIndex(entity => entity.Code).IsUnique();
            modelBuilder.Entity<Customer>().OwnsOne(entity => entity.Address);
            modelBuilder.Entity<Country>().HasKey(entity => entity.Id);
            modelBuilder.Entity<Country>().HasIndex(entity => entity.Code).IsUnique();
            modelBuilder.Entity<ReportRow>().HasNoKey();
            modelBuilder.Entity<ExplicitEntity>().HasKey(entity => entity.Id);
            modelBuilder.Entity<ExplicitEntity>().IsMultiTenant();
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyTenantIsolationByDefault();
            // Reapplying must not duplicate tenant properties or index columns.
            modelBuilder.ApplyTenantIsolationByDefault();
        }
    }

    private sealed class Customer
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public Address Address { get; set; } = new();
    }

    private sealed class Address
    {
        public string City { get; set; } = string.Empty;
    }

    private sealed class Country : IGlobalEntity
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
    }

    private sealed class ReportRow
    {
        public int Count { get; set; }
    }

    private sealed class ExplicitEntity
    {
        public int Id { get; set; }
    }
}

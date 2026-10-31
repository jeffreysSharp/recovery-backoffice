using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Shared.Quota;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Shared.UnitTests;

public sealed class AppTenantInfoTests
{
    [Fact]
    public void Constructor_Should_InitializeActiveTenantAndNormalizeConnection_When_ConnectionIsNull()
    {
        var tenant = new AppTenantInfo("tenant-a", "Company A", null, "admin@example.test", "recovery.backoffice");

        Assert.Equal("tenant-a", tenant.Id);
        Assert.Equal("tenant-a", tenant.Identifier);
        Assert.Equal("Company A", tenant.Name);
        Assert.Equal(string.Empty, tenant.ConnectionString);
        Assert.Equal("admin@example.test", tenant.AdminEmail);
        Assert.Equal("recovery.backoffice", tenant.Issuer);
        Assert.True(tenant.IsActive);
    }

    [Theory]
    [InlineData(1, 2026, 2, 28)]
    [InlineData(3, 2026, 4, 30)]
    [InlineData(12, 2027, 1, 31)]
    public void AddValidity_Should_ExtendCalendarMonths_When_MonthsAreProvided(
        int months, int year, int month, int day)
    {
        var original = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        var tenant = new AppTenantInfo("tenant-a", "company-a") { ValidUpto = original };

        tenant.AddValidity(months);

        Assert.Equal(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc), tenant.ValidUpto);
    }

    [Fact]
    public void SetValidity_Should_AcceptLaterDate_When_ValidityIsExtended()
    {
        var original = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var tenant = new AppTenantInfo("tenant-a", "company-a") { ValidUpto = original };

        tenant.SetValidity(original.AddMonths(2));

        Assert.Equal(original.AddMonths(2), tenant.ValidUpto);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SetValidity_Should_RejectNonIncreasingDate_When_NewDateIsNotLater(int dayOffset)
    {
        var original = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var tenant = new AppTenantInfo("tenant-a", "company-a") { ValidUpto = original };

        Assert.Throws<InvalidOperationException>(() => tenant.SetValidity(original.AddDays(dayOffset)));
        Assert.Equal(original, tenant.ValidUpto);
    }

    [Fact]
    public void Activation_Should_AllowBothTransitions_When_TenantIsNotRoot()
    {
        var tenant = new AppTenantInfo("tenant-a", "company-a");

        tenant.Activate();
        Assert.True(tenant.IsActive);

        tenant.Deactivate();
        Assert.False(tenant.IsActive);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Activation_Should_RejectStateChange_When_TenantIsRoot(bool activate)
    {
        var tenant = new AppTenantInfo(MultitenancyConstants.Root.Id, "root") { IsActive = !activate };

        if (activate)
        {
            Assert.Throws<InvalidOperationException>(() => tenant.Activate());
        }
        else
        {
            Assert.Throws<InvalidOperationException>(() => tenant.Deactivate());
        }

        Assert.Equal(!activate, tenant.IsActive);
    }

    [Fact]
    public void ConnectionString_Should_RejectNull_When_SetThroughTenantContract()
    {
        var tenant = new AppTenantInfo("tenant-a", "company-a") { ConnectionString = "Server=localhost" };
        IAppTenantInfo contract = tenant;

        Assert.Throws<InvalidOperationException>(() => contract.ConnectionString = null);
        Assert.Equal("Server=localhost", tenant.ConnectionString);
    }

    [Fact]
    public void QuotaLimits_Should_RemainIndependent_When_TenantsAreDifferent()
    {
        var first = new AppTenantInfo("tenant-a", "company-a");
        var second = new AppTenantInfo("tenant-b", "company-b");

        first.QuotaLimits[QuotaResource.Users] = 10;

        Assert.Equal(10L, first.QuotaLimits[QuotaResource.Users]);
        Assert.Empty(second.QuotaLimits);
    }
}

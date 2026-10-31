using Recovery.BackOffice.Identity.Domain;
using Recovery.BackOffice.Identity.Domain.Events;
using Xunit;

namespace Recovery.BackOffice.Identity.UnitTests;

public sealed class RecoveryUserTests
{
    [Fact]
    public void RecordRegistered_Should_CaptureUserAndTenant_When_RegistrationIsRecorded()
    {
        var user = new RecoveryUser { Id = "user-a", Email = "user@example.test", FirstName = "Ana", LastName = "Silva" };

        user.RecordRegistered("company-a");

        var domainEvent = Assert.IsType<UserRegisteredEvent>(Assert.Single(user.DomainEvents));
        Assert.Equal(user.Id, domainEvent.UserId);
        Assert.Equal(user.Email, domainEvent.Email);
        Assert.Equal("Ana", domainEvent.FirstName);
        Assert.Equal("Silva", domainEvent.LastName);
        Assert.Equal("company-a", domainEvent.TenantId);
        Assert.NotEqual(Guid.Empty, domainEvent.EventId);
        Assert.Equal(TimeSpan.Zero, domainEvent.OccurredOnUtc.Offset);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RecordPasswordChanged_Should_PreserveResetFlag_When_ChangeIsRecorded(bool wasReset)
    {
        var user = new RecoveryUser { Id = "user-a" };

        user.RecordPasswordChanged(wasReset, "company-a");

        var domainEvent = Assert.IsType<PasswordChangedEvent>(Assert.Single(user.DomainEvents));
        Assert.Equal(user.Id, domainEvent.UserId);
        Assert.Equal(wasReset, domainEvent.WasReset);
        Assert.Equal("company-a", domainEvent.TenantId);
    }

    [Fact]
    public void Activate_Should_RecordOnlyOneEvent_When_CalledTwice()
    {
        var user = new RecoveryUser { Id = "user-a" };

        user.Activate("operator-a", "company-a");
        user.Activate("operator-b", "company-b");

        Assert.True(user.IsActive);
        var domainEvent = Assert.IsType<UserActivatedEvent>(Assert.Single(user.DomainEvents));
        Assert.Equal(user.Id, domainEvent.UserId);
        Assert.Equal("operator-a", domainEvent.ActivatedBy);
        Assert.Equal("company-a", domainEvent.TenantId);
    }

    [Fact]
    public void Deactivate_Should_RecordReasonOnce_When_ActiveUserIsDeactivatedTwice()
    {
        var user = new RecoveryUser { Id = "user-a", IsActive = true };

        user.Deactivate("operator-a", "Requested by administrator", "company-a");
        user.Deactivate("operator-b");

        Assert.False(user.IsActive);
        var domainEvent = Assert.IsType<UserDeactivatedEvent>(Assert.Single(user.DomainEvents));
        Assert.Equal(user.Id, domainEvent.UserId);
        Assert.Equal("operator-a", domainEvent.DeactivatedBy);
        Assert.Equal("Requested by administrator", domainEvent.Reason);
        Assert.Equal("company-a", domainEvent.TenantId);
    }

    [Fact]
    public void RecordRolesAssigned_Should_SnapshotRoles_When_OriginalListIsChanged()
    {
        var user = new RecoveryUser { Id = "user-a" };
        var roles = new List<string> { "Operator" };

        user.RecordRolesAssigned(roles, "company-a");
        roles.Add("Administrator");

        var domainEvent = Assert.IsType<UserRoleAssignedEvent>(Assert.Single(user.DomainEvents));
        Assert.Equal(user.Id, domainEvent.UserId);
        Assert.Equal("company-a", domainEvent.TenantId);
        Assert.Equal("Operator", Assert.Single(domainEvent.AssignedRoles));
    }

    [Fact]
    public void RecordRolesAssigned_Should_LeaveEventsEmpty_When_NoRolesAreAssigned()
    {
        var user = new RecoveryUser();

        user.RecordRolesAssigned([]);

        Assert.Empty(user.DomainEvents);
    }

    [Fact]
    public void ClearDomainEvents_Should_KeepUserState_When_EventsAreCleared()
    {
        var user = new RecoveryUser();
        user.Activate();

        user.ClearDomainEvents();

        Assert.Empty(user.DomainEvents);
        Assert.True(user.IsActive);
    }

    [Theory]
    [InlineData("operator", "OPERATOR")]
    [InlineData("Admin", "ADMIN")]
    public void RoleConstructor_Should_NormalizeName_When_RoleIsCreated(string name, string normalized)
    {
        var role = new RecoveryRole(name, "Description");

        Assert.Equal(name, role.Name);
        Assert.Equal(normalized, role.NormalizedName);
        Assert.Equal("Description", role.Description);
    }

    [Fact]
    public void PasswordHistoryCreate_Should_PreserveHashAndUser_When_HistoryIsRecorded()
    {
        var before = DateTime.UtcNow;

        var history = PasswordHistory.Create("user-a", "test-hash");

        Assert.Equal("user-a", history.UserId);
        Assert.Equal("test-hash", history.PasswordHash);
        Assert.InRange(history.CreatedAt, before, DateTime.UtcNow);
        Assert.Equal(DateTimeKind.Utc, history.CreatedAt.Kind);
    }
}

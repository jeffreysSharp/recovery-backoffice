using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.BuildingBlocks.Shared.Constants;
using Recovery.BackOffice.Identity.Contracts.Services;
using Xunit;

namespace Recovery.BackOffice.Identity.UnitTests;

public sealed class CurrentUserServiceTests
{
    [Fact]
    public void CurrentUser_Should_ReadIdentityAndClaims_When_PrincipalIsAuthenticated()
    {
        // Arrange
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var user = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        var id = Guid.NewGuid();
        var principal = CreatePrincipal(id.ToString());

        // Act
        user.SetCurrentUser(principal);

        // Assert
        Assert.True(user.IsAuthenticated());
        Assert.Equal(id, user.GetUserId());
        Assert.Equal("Recovery User", user.Name);
        Assert.Equal("user@example.test", user.GetUserEmail());
        Assert.Equal("tenant-a", user.GetTenant());
        Assert.True(user.IsInRole("Administrator"));
        Assert.False(user.IsInRole("UnknownRole"));
        Assert.Equal(principal.Claims, user.GetUserClaims());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CurrentUser_Should_ReturnAnonymousDefaults_When_NoAuthenticatedPrincipalExists(bool initializeAnonymous)
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var user = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        if (initializeAnonymous)
        {
            user.SetCurrentUser(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        Assert.False(user.IsAuthenticated());
        Assert.Equal(Guid.Empty, user.GetUserId());
        Assert.Equal(string.Empty, user.GetUserEmail());
        Assert.Equal(string.Empty, user.GetTenant());
        Assert.False(user.IsInRole("Administrator"));
    }

    [Fact]
    public void SetCurrentUser_Should_RejectReinitialization_When_PrincipalWasAlreadyAssigned()
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var user = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        var id = Guid.NewGuid();
        user.SetCurrentUser(CreatePrincipal(id.ToString()));

        Assert.Throws<CustomException>(() => user.SetCurrentUser(CreatePrincipal(Guid.NewGuid().ToString())));
        Assert.Equal(id, user.GetUserId());
    }

    [Fact]
    public void SetCurrentUserId_Should_ProvideFallbackIdentity_When_NoPrincipalIsAuthenticated()
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var user = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        var id = Guid.NewGuid();

        user.SetCurrentUserId(id.ToString());

        Assert.Equal(id, user.GetUserId());
        Assert.False(user.IsAuthenticated());
        Assert.Throws<CustomException>(() => user.SetCurrentUserId(Guid.NewGuid().ToString()));
    }

    [Fact]
    public void GetUserId_Should_PreferAuthenticatedPrincipal_When_FallbackIdentityExists()
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var user = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        user.SetCurrentUserId(Guid.NewGuid().ToString());
        var principalId = Guid.NewGuid();

        user.SetCurrentUser(CreatePrincipal(principalId.ToString()));

        Assert.Equal(principalId, user.GetUserId());
    }

    [Fact]
    public void GetUserId_Should_RejectMalformedIdentifier_When_AuthenticatedClaimIsNotGuid()
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var user = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        user.SetCurrentUser(CreatePrincipal("invalid-id"));

        Assert.Throws<FormatException>(() => user.GetUserId());
    }

    [Fact]
    public void CurrentUser_Should_ShareAliasesAndIsolateUsers_When_ScopesDiffer()
    {
        using var host = CreateHost();
        using var firstScope = host.Services.CreateScope();
        using var secondScope = host.Services.CreateScope();
        var user = firstScope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        var id = Guid.NewGuid();
        firstScope.ServiceProvider.GetRequiredService<ICurrentUserInitializer>()
            .SetCurrentUser(CreatePrincipal(id.ToString()));

        Assert.Same(user, firstScope.ServiceProvider.GetRequiredService<ICurrentUser>());
        Assert.Same(user, firstScope.ServiceProvider.GetRequiredService<ICurrentUserInitializer>());
        Assert.Equal(id, firstScope.ServiceProvider.GetRequiredService<ICurrentUser>().GetUserId());
        Assert.Equal(Guid.Empty, secondScope.ServiceProvider.GetRequiredService<ICurrentUser>().GetUserId());
    }

    private static ClaimsPrincipal CreatePrincipal(string id) => new(new ClaimsIdentity(
        [new Claim(ClaimTypes.NameIdentifier, id),
         new Claim(ClaimTypes.Name, "Recovery User"),
         new Claim(ClaimTypes.Email, "user@example.test"),
         new Claim(CustomClaims.Tenant, "tenant-a"),
         new Claim(ClaimTypes.Role, "Administrator")], "TestAuthentication"));

    private static IHost CreateHost()
    {
        var builder = Host.CreateApplicationBuilder();
        new IdentityModule().ConfigureServices(builder);
        return builder.Build();
    }
}

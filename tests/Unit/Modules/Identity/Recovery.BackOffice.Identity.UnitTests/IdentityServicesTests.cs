using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Identity.Domain;
using Xunit;

namespace Recovery.BackOffice.Identity.UnitTests;

public sealed class IdentityServicesTests
{
    [Theory]
    [InlineData("Abcdefghi1", null)]
    [InlineData("Abcdefghi1!", null)]
    [InlineData("Abcdefgh1", "PasswordTooShort")]
    [InlineData("Abcdefghij", "PasswordRequiresDigit")]
    [InlineData("abcdefghij1", "PasswordRequiresUpper")]
    [InlineData("ABCDEFGHIJ1", "PasswordRequiresLower")]
    public async Task PasswordValidator_Should_ApplyModelPolicy_When_PasswordIsValidated(string password, string? errorCode)
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<RecoveryUser>>();
        var validator = Assert.Single(manager.PasswordValidators);

        var result = await validator.ValidateAsync(manager, new RecoveryUser(), password);

        Assert.Equal(errorCode is null, result.Succeeded);
        if (errorCode is not null) Assert.Contains(result.Errors, error => error.Code == errorCode);
    }

    [Fact]
    public void Options_Should_RequireUniqueEmailsAndConfigureLockout_When_IdentityIsRegistered()
    {
        using var host = CreateHost();
        var options = host.Services.GetRequiredService<IOptions<IdentityOptions>>().Value;

        Assert.True(options.User.RequireUniqueEmail);
        Assert.True(options.Lockout.AllowedForNewUsers);
        Assert.Equal(5, options.Lockout.MaxFailedAccessAttempts);
        Assert.Equal(TimeSpan.FromMinutes(15), options.Lockout.DefaultLockoutTimeSpan);
    }

    [Fact]
    public void Managers_Should_UseScopedEfStores_When_ResolvedFromContainer()
    {
        using var host = CreateHost();
        using var firstScope = host.Services.CreateScope();
        using var secondScope = host.Services.CreateScope();
        var users = firstScope.ServiceProvider.GetRequiredService<UserManager<RecoveryUser>>();
        var roles = firstScope.ServiceProvider.GetRequiredService<RoleManager<RecoveryRole>>();
        var userStore = firstScope.ServiceProvider.GetRequiredService<IUserStore<RecoveryUser>>();
        var roleStore = firstScope.ServiceProvider.GetRequiredService<IRoleStore<RecoveryRole>>();

        Assert.Same(users, firstScope.ServiceProvider.GetRequiredService<UserManager<RecoveryUser>>());
        Assert.NotSame(users, secondScope.ServiceProvider.GetRequiredService<UserManager<RecoveryUser>>());
        Assert.Same(roles, firstScope.ServiceProvider.GetRequiredService<RoleManager<RecoveryRole>>());
        Assert.NotSame(roles, secondScope.ServiceProvider.GetRequiredService<RoleManager<RecoveryRole>>());
        Assert.IsAssignableFrom<IUserRoleStore<RecoveryUser>>(userStore);
        Assert.IsAssignableFrom<IUserPasswordStore<RecoveryUser>>(userStore);
        Assert.IsAssignableFrom<IUserLockoutStore<RecoveryUser>>(userStore);
        Assert.IsAssignableFrom<IRoleClaimStore<RecoveryRole>>(roleStore);
        Assert.Contains("[identity].[Users]", users.Users.ToQueryString());
        Assert.Contains("[TenantId] =", users.Users.ToQueryString());
        Assert.Contains("company-a", users.Users.ToQueryString());
        Assert.Contains("[identity].[Roles]", roles.Roles.ToQueryString());
        Assert.Contains("[TenantId] =", roles.Roles.ToQueryString());
    }

    [Fact]
    public void PasswordHasher_Should_RejectDifferentPassword_When_HashIsVerified()
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<RecoveryUser>>();
        var user = new RecoveryUser();

        var hash = manager.PasswordHasher.HashPassword(user, "Abcdefghi1");

        Assert.NotEqual("Abcdefghi1", hash);
        Assert.Equal(PasswordVerificationResult.Success,
            manager.PasswordHasher.VerifyHashedPassword(user, hash, "Abcdefghi1"));
        Assert.Equal(PasswordVerificationResult.Failed,
            manager.PasswordHasher.VerifyHashedPassword(user, hash, "DifferentPassword1"));
    }

    [Fact]
    public async Task EmailToken_Should_BeBoundToUserAndPurpose_When_DefaultProviderIsUsed()
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<RecoveryUser>>();
        var user = new RecoveryUser { Id = "user-a", SecurityStamp = "stamp-a" };
        var otherUser = new RecoveryUser { Id = "user-b", SecurityStamp = "stamp-b" };

        var token = await manager.GenerateEmailConfirmationTokenAsync(user);
        var provider = manager.Options.Tokens.EmailConfirmationTokenProvider;
        var tampered = (token[0] == 'A' ? 'B' : 'A') + token[1..];

        Assert.True(await manager.VerifyUserTokenAsync(user, provider, "EmailConfirmation", token));
        Assert.False(await manager.VerifyUserTokenAsync(otherUser, provider, "EmailConfirmation", token));
        Assert.False(await manager.VerifyUserTokenAsync(user, provider, "ResetPassword", token));
        Assert.False(await manager.VerifyUserTokenAsync(user, provider, "EmailConfirmation", tampered));
        user.SecurityStamp = "changed-stamp";
        Assert.False(await manager.VerifyUserTokenAsync(user, provider, "EmailConfirmation", token));
    }

    private static IHost CreateHost()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DatabaseOptions:Provider"] = "MSSQL",
            ["DatabaseOptions:ConnectionString"] = "Server=localhost;Database=IdentityServicesTests;Integrated Security=True",
            ["DatabaseOptions:MigrationsAssembly"] = typeof(IdentityServicesTests).Assembly.GetName().Name
        });
        new IdentityModule().ConfigureServices(builder);
        builder.Services.AddRecoveryDatabaseOptions(builder.Configuration);
        builder.Services.AddScoped<IMultiTenantContextAccessor<AppTenantInfo>>(_ =>
        {
            var accessor = new AsyncLocalMultiTenantContextAccessor<AppTenantInfo>();
            ((IMultiTenantContextSetter)accessor).MultiTenantContext = new MultiTenantContext<AppTenantInfo>(
                new AppTenantInfo("company-a", "company-a", "Company A"));
            return accessor;
        });
        builder.Services.AddRecoveryIdentity();
        // Test keys exist only in memory; production key persistence is a deployment concern.
        builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
        return builder.Build();
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.Identity.Data;
using Recovery.BackOffice.Identity.Domain;

namespace Recovery.BackOffice.Identity;

public static class IdentityServiceCollectionExtensions
{
    public static IServiceCollection AddRecoveryIdentity(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddRecoveryDbContext<IdentityDbContext>();
        services.AddIdentity<RecoveryUser, RecoveryRole>(options =>
        {
            options.Password.RequiredLength = IdentityModuleConstants.PasswordLength;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.User.RequireUniqueEmail = true;
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}

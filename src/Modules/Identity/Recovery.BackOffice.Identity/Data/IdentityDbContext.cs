using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Identity.Domain;

namespace Recovery.BackOffice.Identity.Data;

public class IdentityDbContext : MultiTenantIdentityDbContext<RecoveryUser,
    RecoveryRole,
    string,
    IdentityUserClaim<string>,
    IdentityUserRole<string>,
    IdentityUserLogin<string>,
    RecoveryRoleClaim,
    IdentityUserToken<string>,
    IdentityUserPasskey<string>>
{
    private readonly DatabaseOptions _settings;
    private readonly AppTenantInfo? _tenantInfo;
    private readonly IHostEnvironment _environment;

    public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();

    public IdentityDbContext(
        IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
        DbContextOptions<IdentityDbContext> options,
        IOptions<DatabaseOptions> settings,
        IHostEnvironment environment) : base(multiTenantContextAccessor, options)
    {
        ArgumentNullException.ThrowIfNull(multiTenantContextAccessor);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(environment);

        _settings = settings.Value;
        _tenantInfo = multiTenantContextAccessor.MultiTenantContext.TenantInfo;
        _environment = environment;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        builder.ApplyTenantIsolationByDefault();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        if (string.IsNullOrWhiteSpace(_tenantInfo?.ConnectionString)) return;

        var connectionProvider = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()?
            .ApplicationServiceProvider?.GetService<IScopedDbConnectionProvider>();
        if (connectionProvider is null)
        {
            optionsBuilder.ConfigureRecoveryDatabase(_settings.Provider, _tenantInfo.ConnectionString,
                _settings.MigrationsAssembly, _environment.IsDevelopment());
            return;
        }

        optionsBuilder.ConfigureRecoveryDatabase(_settings.Provider,
            connectionProvider.GetConnection(_settings.Provider, _tenantInfo.ConnectionString),
            _settings.MigrationsAssembly, _environment.IsDevelopment());
    }
}

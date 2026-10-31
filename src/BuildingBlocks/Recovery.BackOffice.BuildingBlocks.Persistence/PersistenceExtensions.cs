using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;

namespace Recovery.BackOffice.BuildingBlocks.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddRecoveryDatabaseOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(nameof(DatabaseOptions)))
            .ValidateDataAnnotations()
            .Validate(options => !string.IsNullOrWhiteSpace(options.Provider),
                "DatabaseOptions.Provider is required.")
            .ValidateOnStart();

        services.TryAddScoped<IScopedDbConnectionProvider, ScopedDbConnectionProvider>();

        return services;
    }

    public static IServiceCollection AddRecoveryDbContext<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddDbContext<TContext>((provider, options) =>
        {
            var environment = provider.GetRequiredService<IHostEnvironment>();
            var settings = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var connection = provider.GetRequiredService<IScopedDbConnectionProvider>()
                .GetConnection(settings.Provider, settings.ConnectionString);

            options.ConfigureRecoveryDatabase(settings.Provider, connection,
                settings.MigrationsAssembly, environment.IsDevelopment());
        });

        return services;
    }
}

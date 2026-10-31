using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
}

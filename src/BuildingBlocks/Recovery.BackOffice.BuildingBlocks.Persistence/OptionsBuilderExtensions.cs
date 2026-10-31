using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;

namespace Recovery.BackOffice.BuildingBlocks.Persistence;

public static class OptionsBuilderExtensions
{
    public static DbContextOptionsBuilder ConfigureRecoveryDatabase(
        this DbContextOptionsBuilder builder,
        string dbProvider,
        string connectionString,
        string migrationsAssembly,
        bool isDevelopment)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(dbProvider);

        switch (dbProvider.ToUpperInvariant())
        {
            case DbProviders.MSSQL:
                builder.UseSqlServer(connectionString, options =>
                {
                    options.MigrationsAssembly(migrationsAssembly);
                    options.EnableRetryOnFailure();
                });
                break;

            default:
                throw new InvalidOperationException($"Database provider {dbProvider} is not supported.");
        }

        if (isDevelopment)
        {
            builder.EnableSensitiveDataLogging();
            builder.EnableDetailedErrors();
        }

        return builder;
    }
}

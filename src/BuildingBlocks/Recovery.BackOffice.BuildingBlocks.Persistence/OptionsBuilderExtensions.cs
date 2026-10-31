using System.Data.Common;
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

        ConfigureDiagnostics(builder, isDevelopment);

        return builder;
    }

    public static DbContextOptionsBuilder ConfigureRecoveryDatabase(
        this DbContextOptionsBuilder builder,
        string dbProvider,
        DbConnection connection,
        string migrationsAssembly,
        bool isDevelopment)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(dbProvider);

        switch (dbProvider.ToUpperInvariant())
        {
            case DbProviders.MSSQL:
                builder.UseSqlServer(connection, contextOwnsConnection: false, options =>
                {
                    options.MigrationsAssembly(migrationsAssembly);
                    options.EnableRetryOnFailure();
                });
                break;

            default:
                throw new InvalidOperationException($"Database provider {dbProvider} is not supported.");
        }

        ConfigureDiagnostics(builder, isDevelopment);

        return builder;
    }

    private static void ConfigureDiagnostics(DbContextOptionsBuilder builder, bool isDevelopment)
    {
        if (isDevelopment)
        {
            builder.EnableSensitiveDataLogging();
            builder.EnableDetailedErrors();
        }
    }
}

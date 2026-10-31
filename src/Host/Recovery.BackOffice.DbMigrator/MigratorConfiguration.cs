using Microsoft.Extensions.Configuration;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Migrations.SqlServer;

namespace Recovery.BackOffice.DbMigrator;

public static class MigratorConfiguration
{
    public static DatabaseOptions Read(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        var settings = configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>() ?? new DatabaseOptions();
        if (!string.Equals(settings.Provider, DbProviders.MSSQL, StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Only MSSQL is supported by Recovery DbMigrator.");
        }
        if (string.IsNullOrWhiteSpace(settings.ConnectionString))
        {
            throw new InvalidOperationException("DatabaseOptions:ConnectionString is required.");
        }

        settings.MigrationsAssembly = typeof(MigrationsAssemblyMarker).Assembly.GetName().Name!;
        return settings;
    }
}

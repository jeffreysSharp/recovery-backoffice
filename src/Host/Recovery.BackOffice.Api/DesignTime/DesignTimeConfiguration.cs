using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Migrations.SqlServer;

namespace Recovery.BackOffice.Api.DesignTime;

internal static class DesignTimeConfiguration
{
    internal static DatabaseOptions Read(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
        var provider = configuration["DatabaseOptions:Provider"] ?? DbProviders.MSSQL;
        if (!string.Equals(provider, DbProviders.MSSQL, StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Only MSSQL is supported for Recovery migrations.");
        }

        return new DatabaseOptions
        {
            Provider = DbProviders.MSSQL,
            ConnectionString = configuration["DatabaseOptions:ConnectionString"]
                ?? @"Server=(localdb)\MSSQLLocalDB;Database=RecoveryBackOffice;Integrated Security=True;TrustServerCertificate=True",
            MigrationsAssembly = typeof(MigrationsAssemblyMarker).Assembly.GetName().Name!
        };
    }
}

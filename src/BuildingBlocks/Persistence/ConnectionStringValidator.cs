using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Recovery.BackOffice.BuildingBlocks.Persistence;

/// <summary>
/// Validates database connection strings for supported providers (SQL Server, SQL Server).
/// </summary>
/// <param name="dbSettings">Database configuration options.</param>
/// <param name="logger">Logger instance for error tracking.</param>
public sealed class ConnectionStringValidator(IOptions<DatabaseOptions> dbSettings, ILogger<ConnectionStringValidator> logger) : IConnectionStringValidator
{
    private readonly DatabaseOptions _dbSettings = dbSettings.Value;
    private readonly ILogger<ConnectionStringValidator> _logger = logger;

    public bool TryValidate(string connectionString, string? dbProvider = null)
    {
        if (string.IsNullOrWhiteSpace(dbProvider))
        {
            dbProvider = _dbSettings.Provider;
        }

        try
        {
            switch (dbProvider?.ToUpperInvariant())
            {
                case DbProviders.MSSQL:
                    _ = new SqlConnectionStringBuilder(connectionString);
                    break;
                default:
                    return false;
            }

            return true;
        }
        catch (ArgumentException ex)
        {
            // Catches invalid connection string format from both SqlConnectionStringBuilder
            // and SqlConnectionStringBuilder (both throw ArgumentException for malformed strings).
            _logger.LogError(ex, "Connection String Validation Exception : {Error}", ex.Message);
            return false;
        }
        catch (FormatException ex)
        {
            // Catches format-related parsing failures in connection string values.
            _logger.LogError(ex, "Connection String Validation Exception : {Error}", ex.Message);
            return false;
        }
    }
}
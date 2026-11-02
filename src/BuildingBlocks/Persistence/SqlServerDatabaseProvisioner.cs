using Microsoft.Data.SqlClient;

namespace Recovery.BackOffice.BuildingBlocks.Persistence;

/// <summary>Creates missing SQL Server databases locally; Azure SQL databases must be provisioned separately.</summary>
public static class SqlServerDatabaseProvisioner
{
    public static async Task EnsureExistsAsync(string connectionString, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        var builder = new SqlConnectionStringBuilder(connectionString);
        var database = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(database) || database.Equals("master", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Configure an application database, not master.");
        }

        await using var target = new SqlConnection(connectionString);
        try
        {
            await target.OpenAsync(cancellationToken).ConfigureAwait(false);
            return;
        }
        catch (SqlException exception) when (exception.Number == 4060)
        {
            // Use the original credentials: an already-opened shared SqlConnection hides its password.
        }

        builder.InitialCatalog = "master";
        await using var master = new SqlConnection(builder.ConnectionString);
        await master.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var edition = master.CreateCommand();
        edition.CommandText = "SELECT CONVERT(int, SERVERPROPERTY('EngineEdition'))";
        if (Convert.ToInt32(await edition.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false),
                System.Globalization.CultureInfo.InvariantCulture) == 5)
        {
            throw new InvalidOperationException("Provision the Azure SQL database before provisioning the agency.");
        }

        await using var command = master.CreateCommand();
#pragma warning disable CA2100, S2077 // SQL identifiers cannot be parameters; bracket escaping quotes the complete identifier.
        command.CommandText = "IF DB_ID(@database) IS NULL CREATE DATABASE [" + database.Replace("]", "]]", StringComparison.Ordinal) + "]";
#pragma warning restore CA2100, S2077
        command.Parameters.AddWithValue("@database", database);
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        // A failed open caches the missing-database error in this pool. Drop that cached failure
        // and verify readiness before EF checks existence, otherwise it can try to create it again.
        SqlConnection.ClearPool(target);
        await target.OpenAsync(cancellationToken).ConfigureAwait(false);
    }
}

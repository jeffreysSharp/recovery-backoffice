using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Recovery.BackOffice.DbMigrator;

/// <summary>Database-scoped migration coordination, compatible with Azure SQL.</summary>
internal static class SqlServerMigratorLock
{
    public static async Task WaitForDatabaseAsync(string connectionString, ILogger logger, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(logger);
        var deadline = DateTime.UtcNow.AddMinutes(2);
        while (true)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(ct).ConfigureAwait(false);
                return;
            }
            catch (SqlException ex) when (ex.Number == 4060)
            {
                await CreateLocalDatabaseAsync(connectionString, ct).ConfigureAwait(false);
            }
            catch (SqlException) when (DateTime.UtcNow < deadline)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), ct).ConfigureAwait(false);
            }
            if (DateTime.UtcNow >= deadline)
            {
                throw new TimeoutException("SQL Server did not become available within two minutes.");
            }
        }
    }

    private static async Task CreateLocalDatabaseAsync(string connectionString, CancellationToken ct)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var database = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(database) || database.Equals("master", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Configure an application database, not master.");
        }
        builder.InitialCatalog = "master";
        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync(ct).ConfigureAwait(false);
        await using var edition = connection.CreateCommand();
        edition.CommandText = "SELECT CONVERT(int, SERVERPROPERTY('EngineEdition'))";
        var engine = Convert.ToInt32(await edition.ExecuteScalarAsync(ct).ConfigureAwait(false), CultureInfo.InvariantCulture);
        if (engine == 5)
        {
            throw new InvalidOperationException("Provision the Azure SQL database before running migrations.");
        }
        await using var command = connection.CreateCommand();
        // Database identifiers cannot be parameters. Escape the closing bracket before quoting.
#pragma warning disable CA2100, S2077
        command.CommandText = "IF DB_ID(@database) IS NULL CREATE DATABASE [" + database.Replace("]", "]]", StringComparison.Ordinal) + "]";
#pragma warning restore CA2100, S2077
        command.Parameters.AddWithValue("@database", database);
        await command.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope",
        Justification = "Ownership transfers to the returned IAsyncDisposable on success; the physical connection is disposed on every failure path.")]
    public static async Task<IAsyncDisposable> AcquireAsync(string connectionString, ILogger logger, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(logger);
        // No pooling: closing the physical session also releases the lock after a failed cleanup.
        var builder = new SqlConnectionStringBuilder(connectionString) { Pooling = false };
        var holder = new LockHolder(builder.ConnectionString);
        try
        {
            var connection = holder.Connection;
            await connection.OpenAsync(ct).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandTimeout = 130;
            command.CommandText = """
                DECLARE @result int;
                EXEC @result = sys.sp_getapplock @Resource=N'Recovery.BackOffice.DbMigrator',
                    @LockMode=N'Exclusive', @LockOwner=N'Session', @LockTimeout=120000;
                SELECT @result;
                """;
            var result = Convert.ToInt32(await command.ExecuteScalarAsync(ct).ConfigureAwait(false), CultureInfo.InvariantCulture);
            if (result < 0) throw new TimeoutException("Could not acquire the database migration lock.");
            return holder;
        }
        catch
        {
            await holder.Connection.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    private sealed class LockHolder(string connectionString) : IAsyncDisposable
    {
        public SqlConnection Connection { get; } = new(connectionString);

        public async ValueTask DisposeAsync()
        {
            try
            {
                await using var command = Connection.CreateCommand();
                command.CommandText = "EXEC sys.sp_releaseapplock @Resource=N'Recovery.BackOffice.DbMigrator', @LockOwner=N'Session'";
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            finally
            {
                await Connection.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}

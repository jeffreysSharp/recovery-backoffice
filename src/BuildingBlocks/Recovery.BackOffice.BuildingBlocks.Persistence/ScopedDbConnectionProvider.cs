using System.Data.Common;
using Microsoft.Data.SqlClient;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;

namespace Recovery.BackOffice.BuildingBlocks.Persistence;

public sealed class ScopedDbConnectionProvider : IScopedDbConnectionProvider, IDisposable, IAsyncDisposable
{
    private readonly Dictionary<string, DbConnection> _connections = new(StringComparer.Ordinal);
    private bool _disposed;

    public DbConnection GetConnection(string dbProvider, string connectionString)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(dbProvider);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!string.Equals(dbProvider, DbProviders.MSSQL, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Database provider {dbProvider} is not supported.");
        }

        if (_connections.TryGetValue(connectionString, out var existing))
        {
            return existing;
        }

        var connection = new SqlConnection(connectionString);
        _connections.Add(connectionString, connection);

        return connection;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        foreach (var connection in _connections.Values)
        {
            connection.Dispose();
        }

        _connections.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        foreach (var connection in _connections.Values)
        {
            await connection.DisposeAsync().ConfigureAwait(false);
        }

        _connections.Clear();
    }
}

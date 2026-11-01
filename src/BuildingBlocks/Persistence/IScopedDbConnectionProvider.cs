using System.Data.Common;

namespace Recovery.BackOffice.BuildingBlocks.Persistence;

/// <summary>
/// One <see cref="DbConnection"/> per connection string per DI scope, shared by every DbContext in
/// that scope.
///
/// Required for cross-context transactions: EF Core can enlist a second context in an existing
/// transaction only when both contexts share the physical connection. Otherwise the outbox uses a
/// separate transaction from the business write it is supposed to accompany.
/// </summary>
public interface IScopedDbConnectionProvider
{
    /// <summary>
    /// Returns the scope's connection for <paramref name="connectionString"/>, creating it on
    /// first use. The connection is owned by the scope, not by any DbContext.
    /// </summary>
    DbConnection GetConnection(string dbProvider, string connectionString);
}

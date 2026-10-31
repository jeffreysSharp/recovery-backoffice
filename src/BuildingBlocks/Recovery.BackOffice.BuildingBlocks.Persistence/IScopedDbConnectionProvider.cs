using System.Data.Common;

namespace Recovery.BackOffice.BuildingBlocks.Persistence;

public interface IScopedDbConnectionProvider
{
    DbConnection GetConnection(string dbProvider, string connectionString);
}

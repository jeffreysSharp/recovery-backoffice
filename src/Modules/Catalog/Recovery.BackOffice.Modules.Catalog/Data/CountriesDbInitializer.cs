using Recovery.BackOffice.BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Recovery.BackOffice.Modules.Catalog.Data;

public sealed class CountriesDbInitializer(CountriesDbContext dbContext, ILogger<CountriesDbInitializer> logger) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[Countries] applied central catalog migrations");
        }
    }
    public Task SeedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

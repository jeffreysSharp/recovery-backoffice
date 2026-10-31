using Finbuckle.MultiTenant.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Eventing.Inbox;
using Recovery.BackOffice.BuildingBlocks.Eventing.Outbox;
using Recovery.BackOffice.BuildingBlocks.Persistence.Context;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Recovery.BackOffice.BuildingBlocks.Eventing.Persistence;

/// <summary>
/// The single context owning the transactional outbox and inbox.
///
/// Derives from <see cref="BaseDbContext"/> so it inherits per-tenant connection
/// routing: for a tenant with a dedicated database, the outbox row lands in that
/// same database as the business data it accompanies. That is what makes the
/// outbox transactional on every supported deployment topology.
///
/// Owning these tables here — rather than once per module DbContext — is what
/// keeps <c>IOutboxStore</c>/<c>IInboxStore</c> to a single, unambiguous DI
/// registration (issue #1349).
/// </summary>
public class EventingDbContext : BaseDbContext
{
    public EventingDbContext(
        IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
        DbContextOptions<EventingDbContext> options,
        IOptions<DatabaseOptions> settings,
        IHostEnvironment environment)
        : base(multiTenantContextAccessor, options, settings, environment)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration(EventingConstants.SchemaName));
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration(EventingConstants.SchemaName));

        // Must run last: ApplyTenantIsolationByDefault inspects the configured entities.
        base.OnModelCreating(modelBuilder);
    }
}

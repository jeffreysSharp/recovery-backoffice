using Recovery.BackOffice.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.GetTenantMigrations;

public sealed record GetTenantMigrationsQuery : IQuery<IReadOnlyCollection<TenantMigrationStatusDto>>;
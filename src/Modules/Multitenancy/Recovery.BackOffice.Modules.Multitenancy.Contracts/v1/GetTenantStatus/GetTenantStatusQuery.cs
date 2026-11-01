using Recovery.BackOffice.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.GetTenantStatus;

public sealed record GetTenantStatusQuery(string TenantId) : IQuery<TenantStatusDto>;
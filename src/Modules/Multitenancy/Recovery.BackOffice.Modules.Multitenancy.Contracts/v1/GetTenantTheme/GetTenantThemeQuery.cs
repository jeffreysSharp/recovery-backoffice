using Recovery.BackOffice.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.GetTenantTheme;

public sealed record GetTenantThemeQuery : IQuery<TenantThemeDto>;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.UpdateTenantTheme;

public sealed record UpdateTenantThemeCommand(TenantThemeDto Theme) : ICommand;
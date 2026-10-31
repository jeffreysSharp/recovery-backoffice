using Mediator;

namespace Recovery.BackOffice.Modules.Identity.Contracts.v1.Roles.DeleteRole;

public sealed record DeleteRoleCommand(string Id) : ICommand<Unit>;
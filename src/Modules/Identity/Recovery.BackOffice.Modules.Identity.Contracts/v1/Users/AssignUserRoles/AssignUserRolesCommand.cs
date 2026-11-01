using Recovery.BackOffice.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace Recovery.BackOffice.Modules.Identity.Contracts.v1.Users.AssignUserRoles;

public sealed class AssignUserRolesCommand : ICommand<string>
{
    public required string UserId { get; init; }
    public List<UserRoleDto> UserRoles { get; init; } = new();
}
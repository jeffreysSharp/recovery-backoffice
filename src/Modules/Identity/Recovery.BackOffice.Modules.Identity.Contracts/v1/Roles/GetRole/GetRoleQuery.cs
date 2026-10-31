using Recovery.BackOffice.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace Recovery.BackOffice.Modules.Identity.Contracts.v1.Roles.GetRole;

public sealed record GetRoleQuery(string Id) : IQuery<RoleDto?>;
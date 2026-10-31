using Recovery.BackOffice.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace Recovery.BackOffice.Modules.Identity.Contracts.v1.Users.GetUserGroups;

public sealed record GetUserGroupsQuery(string UserId) : IQuery<IEnumerable<GroupDto>>;
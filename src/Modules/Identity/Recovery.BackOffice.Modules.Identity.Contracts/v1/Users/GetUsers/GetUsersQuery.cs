using Recovery.BackOffice.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace Recovery.BackOffice.Modules.Identity.Contracts.v1.Users.GetUsers;

public sealed record GetUsersQuery : IQuery<List<UserDto>>;
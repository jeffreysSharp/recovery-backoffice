using Recovery.BackOffice.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace Recovery.BackOffice.Modules.Identity.Contracts.v1.Sessions.GetMySessions;

public sealed record GetMySessionsQuery : IQuery<List<UserSessionDto>>;
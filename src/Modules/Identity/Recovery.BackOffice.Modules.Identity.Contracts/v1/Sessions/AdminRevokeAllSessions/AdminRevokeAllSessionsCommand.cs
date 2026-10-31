using Mediator;

namespace Recovery.BackOffice.Modules.Identity.Contracts.v1.Sessions.AdminRevokeAllSessions;

public sealed record AdminRevokeAllSessionsCommand(Guid UserId, string? Reason = null) : ICommand<int>;
using Recovery.BackOffice.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace Recovery.BackOffice.Modules.Identity.Contracts.v1.Impersonation.EndImpersonation;

public sealed record EndImpersonationCommand() : ICommand<TokenResponse>;

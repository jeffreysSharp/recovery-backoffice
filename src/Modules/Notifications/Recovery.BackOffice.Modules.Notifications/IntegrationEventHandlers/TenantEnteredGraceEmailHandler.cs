using Recovery.BackOffice.BuildingBlocks.Eventing.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Mailing.Services;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace Recovery.BackOffice.Modules.Notifications.IntegrationEventHandlers;

/// <summary>Emails the tenant admin that their subscription lapsed and the grace period is counting down.</summary>
public sealed class TenantEnteredGraceEmailHandler(
    IMailService mailService,
    ILogger<TenantEnteredGraceEmailHandler> logger)
    : IIntegrationEventHandler<TenantEnteredGraceIntegrationEvent>
{
    public async Task HandleAsync(TenantEnteredGraceIntegrationEvent @event, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(@event);
        var (subject, body, textBody) = BillingEmailBodies.EnteredGrace(
            @event.TenantName, @event.PlanKey, @event.ValidUpto, @event.GraceEndsUtc);
        await BillingEmailSender.SendAsync(mailService, logger, @event.AdminEmail, subject, body, textBody, "entered-grace", ct)
            .ConfigureAwait(false);
    }
}

using Recovery.BackOffice.BuildingBlocks.Eventing.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Mailing.Services;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace Recovery.BackOffice.Modules.Notifications.IntegrationEventHandlers;

/// <summary>Emails the tenant admin that their subscription is nearing expiry.</summary>
public sealed class TenantNearingExpiryEmailHandler(
    IMailService mailService,
    ILogger<TenantNearingExpiryEmailHandler> logger)
    : IIntegrationEventHandler<TenantNearingExpiryIntegrationEvent>
{
    public async Task HandleAsync(TenantNearingExpiryIntegrationEvent @event, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(@event);
        var (subject, body, textBody) = BillingEmailBodies.NearingExpiry(
            @event.TenantName, @event.PlanKey, @event.ValidUpto, @event.DaysRemaining);
        await BillingEmailSender.SendAsync(mailService, logger, @event.AdminEmail, subject, body, textBody, "nearing-expiry", ct)
            .ConfigureAwait(false);
    }
}

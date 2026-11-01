using Recovery.BackOffice.BuildingBlocks.Eventing.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Mailing.Services;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace Recovery.BackOffice.Modules.Notifications.IntegrationEventHandlers;

/// <summary>Emails the tenant admin that their subscription expired and access is suspended.</summary>
public sealed class TenantExpiredEmailHandler(
    IMailService mailService,
    ILogger<TenantExpiredEmailHandler> logger)
    : IIntegrationEventHandler<TenantExpiredIntegrationEvent>
{
    public async Task HandleAsync(TenantExpiredIntegrationEvent @event, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(@event);
        var (subject, body, textBody) = BillingEmailBodies.Expired(@event.TenantName, @event.PlanKey, @event.ValidUpto);
        await BillingEmailSender.SendAsync(mailService, logger, @event.AdminEmail, subject, body, textBody, "expired", ct)
            .ConfigureAwait(false);
    }
}

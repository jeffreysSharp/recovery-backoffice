using Recovery.BackOffice.Modules.Billing.Contracts.Authorization;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.Modules.Billing.Contracts.v1.Subscriptions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Billing.Features.v1.Subscriptions.GetSubscription;

public static class GetSubscriptionEndpoint
{
    internal static RouteHandlerBuilder MapGetSubscriptionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/subscriptions",
                (string? tenantId, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetSubscriptionQuery(tenantId), ct))
            .WithName("GetSubscription")
            .WithSummary("Get the active subscription for an agency (admin) or the current agency")
            .RequirePermission(BillingPermissions.View);
    }

    internal static RouteHandlerBuilder MapGetMySubscriptionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/subscriptions/me",
                (IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetSubscriptionQuery(null), ct))
            .WithName("GetMySubscription")
            .WithSummary("Get the active subscription for the current agency");
    }
}

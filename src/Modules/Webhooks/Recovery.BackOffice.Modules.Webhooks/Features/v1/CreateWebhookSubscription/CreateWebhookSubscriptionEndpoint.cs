using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Webhooks.Contracts.Authorization;
using Recovery.BackOffice.Modules.Webhooks.Contracts.v1.CreateWebhookSubscription;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Webhooks.Features.v1.CreateWebhookSubscription;

public static class CreateWebhookSubscriptionEndpoint
{
    internal static RouteHandlerBuilder MapCreateWebhookSubscriptionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/subscriptions", async (
            CreateWebhookSubscriptionCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var id = await mediator.Send(command, ct);
            return TypedResults.Created($"/api/v1/webhooks/subscriptions/{id}", id);
        })
        .WithName("CreateWebhookSubscription")
        .WithSummary("Create a webhook subscription")
        .RequirePermission(WebhooksPermissions.Subscriptions.Create)
        .WithIdempotency()
        .Produces<Guid>(StatusCodes.Status201Created);
    }
}

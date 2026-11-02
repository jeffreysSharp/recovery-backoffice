using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Billing.Contracts.Authorization;
using Recovery.BackOffice.Modules.Billing.Contracts.v1.Wallets;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Billing.Features.v1.Wallets.CreateTopupRequest;

public static class CreateTopupRequestEndpoint
{
    internal static RouteHandlerBuilder MapCreateTopupRequestEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/wallet/topup-requests",
                async (CreateTopupRequestCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CreateTopupRequest")
            .WithSummary("Submit a wallet top-up request for the current agency")
            .RequirePermission(BillingPermissions.View)
            .WithIdempotency();
    }
}

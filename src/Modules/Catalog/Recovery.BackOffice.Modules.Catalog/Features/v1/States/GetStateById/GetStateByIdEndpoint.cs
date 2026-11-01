using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.GetStateById;

public static class GetStateByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetStateByIdEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/states/{stateId:guid}", async (Guid stateId, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(new GetStateByIdQuery(stateId), ct)))
        .WithName("GetStateById")
        .WithSummary("Get a state")
        .RequirePermission(CatalogPermissions.States.View);
}

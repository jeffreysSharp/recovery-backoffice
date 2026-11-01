using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.UpdateState;

public static class UpdateStateEndpoint
{
    internal static RouteHandlerBuilder MapUpdateStateEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapPut("/states/{stateId:guid}", async (Guid stateId, UpdateStateCommand body, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(body with { StateId = stateId }, ct)))
        .WithName("UpdateState")
        .WithSummary("Update a state")
        .RequirePermission(CatalogPermissions.States.Update);
}

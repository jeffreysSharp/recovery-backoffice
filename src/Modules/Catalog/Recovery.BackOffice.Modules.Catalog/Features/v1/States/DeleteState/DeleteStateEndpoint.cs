using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.DeleteState;

public static class DeleteStateEndpoint
{
    internal static RouteHandlerBuilder MapDeleteStateEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapDelete("/states/{stateId:guid}", async (Guid stateId, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(new DeleteStateCommand(stateId), ct)))
        .WithName("DeleteState")
        .WithSummary("Delete a state")
        .RequirePermission(CatalogPermissions.States.Delete);
}

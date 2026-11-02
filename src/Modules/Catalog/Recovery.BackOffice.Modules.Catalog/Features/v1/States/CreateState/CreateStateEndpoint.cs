using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.CreateState;

public static class CreateStateEndpoint
{
    internal static RouteHandlerBuilder MapCreateStateEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost("/states", async (CreateStateCommand command, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(command, ct)))
        .WithName("CreateState")
        .WithSummary("Create a state")
        .RequirePermission(CatalogPermissions.States.Create)
        .WithIdempotency();
}

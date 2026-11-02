using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.CreateAgencyType;

public static class CreateAgencyTypeEndpoint
{
    internal static RouteHandlerBuilder MapCreateAgencyTypeEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost("/agency-types", async (CreateAgencyTypeCommand command, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(command, ct)))
        .WithName("CreateAgencyType")
        .WithSummary("CreateAgencyType")
        .RequirePermission(CatalogPermissions.AgencyTypes.Create)
        .WithIdempotency();
}

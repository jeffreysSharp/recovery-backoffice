using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.UpdateAgencyType;

public static class UpdateAgencyTypeEndpoint
{
    internal static RouteHandlerBuilder MapUpdateAgencyTypeEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapPut("/agency-types/{agencyTypeId:guid}", async (Guid agencyTypeId, UpdateAgencyTypeCommand body, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(body with { AgencyTypeId = agencyTypeId }, ct)))
        .WithName("UpdateAgencyType")
        .WithSummary("UpdateAgencyType")
        .RequirePermission(CatalogPermissions.AgencyTypes.Update);
}

using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.DeleteAgencyType;

public static class DeleteAgencyTypeEndpoint
{
    internal static RouteHandlerBuilder MapDeleteAgencyTypeEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapDelete("/agency-types/{agencyTypeId:guid}", async (Guid agencyTypeId, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(new DeleteAgencyTypeCommand(agencyTypeId), ct)))
        .WithName("DeleteAgencyType")
        .WithSummary("DeleteAgencyType")
        .RequirePermission(CatalogPermissions.AgencyTypes.Delete);
}

using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.DeleteCountry;

public static class DeleteCountryEndpoint
{
    internal static RouteHandlerBuilder MapDeleteCountryEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapDelete("/countries/{countryId:guid}", async (Guid countryId, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(new DeleteCountryCommand(countryId), ct)))
        .WithName("DeleteCountry")
        .WithSummary("DeleteCountry")
        .RequirePermission(CatalogPermissions.Countries.Delete);
}

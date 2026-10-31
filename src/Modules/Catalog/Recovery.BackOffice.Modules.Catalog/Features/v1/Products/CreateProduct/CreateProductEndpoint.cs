using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Products;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Products.CreateProduct;

public static class CreateProductEndpoint
{
    internal static RouteHandlerBuilder MapCreateProductEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/products",
                async (CreateProductCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CreateProduct")
            .WithSummary("Create a product")
            .RequirePermission(CatalogPermissions.Products.Create)
            .WithIdempotency();
    }
}

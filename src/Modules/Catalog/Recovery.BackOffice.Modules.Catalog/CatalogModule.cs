using Asp.Versioning;
using Recovery.BackOffice.Modules.Catalog.Features.v1.States.CreateState;
using Recovery.BackOffice.Modules.Catalog.Features.v1.States.UpdateState;
using Recovery.BackOffice.Modules.Catalog.Features.v1.States.DeleteState;
using Recovery.BackOffice.Modules.Catalog.Features.v1.States.GetStateById;
using Recovery.BackOffice.Modules.Catalog.Features.v1.States.SearchStates;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.CreateCountry;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.UpdateCountry;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.DeleteCountry;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.GetCountryById;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.SearchCountries;
using Recovery.BackOffice.BuildingBlocks.Eventing;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Constants;
using Recovery.BackOffice.BuildingBlocks.Web.Modules;
using Recovery.BackOffice.Modules.Catalog.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Data;
using Recovery.BackOffice.Modules.Files.Contracts;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Brands.CreateBrand;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Brands.DeleteBrand;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Brands.GetBrandById;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Brands.ListTrashedBrands;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Brands.RestoreBrand;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Brands.SearchBrands;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Brands.UpdateBrand;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Categories.CreateCategory;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Categories.DeleteCategory;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Categories.GetCategoryById;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Categories.GetCategoryTree;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Categories.ListTrashedCategories;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Categories.RestoreCategory;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Categories.SearchCategories;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Categories.UpdateCategory;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.AddProductImage;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.AdjustProductStock;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.ChangeProductPrice;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.CreateProduct;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.DeleteProduct;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.GetProductById;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.ListTrashedProducts;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.RemoveProductImage;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.ReorderProductImages;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.RestoreProduct;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.SearchProducts;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.SetProductThumbnail;
using Recovery.BackOffice.Modules.Catalog.Features.v1.Products.UpdateProduct;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: RecoveryModule(typeof(Recovery.BackOffice.Modules.Catalog.CatalogModule), 600)]

namespace Recovery.BackOffice.Modules.Catalog;

public sealed class CatalogModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(CatalogPermissions.All);

        builder.Services.AddRecoveryDbContext<CatalogDbContext>();
        builder.Services.AddRecoveryDbContext<CountriesDbContext>();
        builder.Services.AddScoped<IDbInitializer, CountriesDbInitializer>();
        builder.Services.AddScoped<IDbInitializer, StatesDbInitializer>();
        builder.Services.AddScoped<IDbInitializer, CatalogDbInitializer>();

        // OwnerType=Product policy for Files module attachments (product images).
        builder.Services.AddScoped<IFileAccessPolicy, ProductFileAccessPolicy>();

        // Keeps persisted product-image URLs pointing at their FileAsset when Files moves its object.
        builder.Services.AddIntegrationEventHandlers(typeof(CatalogModule).Assembly);

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<CatalogDbContext>(
                name: "db:catalog",
                failureStatus: HealthStatus.Unhealthy);
    }

    public void ConfigureMiddleware(IApplicationBuilder app)
    {
        // No custom middleware needed
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = endpoints
            .MapGroup("api/v{version:apiVersion}/catalog")
            .WithTags("Catalog")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapCreateCountryEndpoint();
        group.MapCreateStateEndpoint();
        group.MapUpdateStateEndpoint();
        group.MapDeleteStateEndpoint();
        group.MapGetStateByIdEndpoint();
        group.MapSearchStatesEndpoint();
        group.MapUpdateCountryEndpoint();
        group.MapDeleteCountryEndpoint();
        group.MapGetCountryByIdEndpoint();
        group.MapSearchCountriesEndpoint();

        // Trash routes registered first so the literal `/trash` segment wins
        // over the catch-all `/{id:guid}`.
        group.MapListTrashedBrandsEndpoint();
        group.MapRestoreBrandEndpoint();
        group.MapCreateBrandEndpoint();
        group.MapUpdateBrandEndpoint();
        group.MapDeleteBrandEndpoint();
        group.MapGetBrandByIdEndpoint();
        group.MapSearchBrandsEndpoint();

        // Category /tree and /trash must be registered before /{categoryId:guid}
        // so the literal routes win.
        group.MapGetCategoryTreeEndpoint();
        group.MapListTrashedCategoriesEndpoint();
        group.MapRestoreCategoryEndpoint();
        group.MapCreateCategoryEndpoint();
        group.MapUpdateCategoryEndpoint();
        group.MapDeleteCategoryEndpoint();
        group.MapGetCategoryByIdEndpoint();
        group.MapSearchCategoriesEndpoint();

        group.MapListTrashedProductsEndpoint();
        group.MapRestoreProductEndpoint();
        group.MapCreateProductEndpoint();
        group.MapUpdateProductEndpoint();
        group.MapDeleteProductEndpoint();
        group.MapChangeProductPriceEndpoint();
        group.MapAdjustProductStockEndpoint();

        // Product images — collection sub-resource under /products/{id}/images.
        group.MapAddProductImageEndpoint();
        group.MapRemoveProductImageEndpoint();
        group.MapSetProductThumbnailEndpoint();
        group.MapReorderProductImagesEndpoint();

        group.MapGetProductByIdEndpoint();
        group.MapSearchProductsEndpoint();
    }
}

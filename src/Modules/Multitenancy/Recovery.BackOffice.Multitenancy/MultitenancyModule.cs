using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Constants;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Web.Modules;
using Recovery.BackOffice.Multitenancy.Data;

[assembly: RecoveryModule(typeof(Recovery.BackOffice.Multitenancy.MultitenancyModule), 200)]

namespace Recovery.BackOffice.Multitenancy;

public sealed class MultitenancyModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddRecoveryDbContext<TenantDbContext>();
        builder.Services.AddMultiTenant<AppTenantInfo>()
            .WithClaimStrategy(CustomClaims.Tenant)
            .WithHeaderStrategy(MultitenancyConstants.Identifier)
            .WithDelegateStrategy(context =>
            {
                if (context is not HttpContext httpContext ||
                    !httpContext.Request.Query.TryGetValue("tenant", out var identifier) ||
                    string.IsNullOrEmpty(identifier))
                {
                    return Task.FromResult<string?>(null);
                }

                return Task.FromResult<string?>(identifier.ToString());
            })
            .WithStore<EFCoreStore<TenantDbContext, AppTenantInfo>>(ServiceLifetime.Scoped);
    }

    public void ConfigureMiddleware(IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
    }
}

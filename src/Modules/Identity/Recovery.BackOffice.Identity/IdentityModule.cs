using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.BuildingBlocks.Web.Modules;
using Recovery.BackOffice.BuildingBlocks.Web.Origin;
using Recovery.BackOffice.Identity.Contracts.Services;
using Recovery.BackOffice.Identity.Services;

[assembly: RecoveryModule(typeof(Recovery.BackOffice.Identity.IdentityModule), 1)]

namespace Recovery.BackOffice.Identity;

public sealed class IdentityModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddOptions<OriginOptions>().BindConfiguration(nameof(OriginOptions));
        builder.Services.AddScoped<IRequestContextService, RequestContextService>();
        builder.Services.AddScoped<IRequestContext>(provider => provider.GetRequiredService<IRequestContextService>());
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddScoped<ICurrentUser>(provider => provider.GetRequiredService<ICurrentUserService>());
        builder.Services.AddScoped<ICurrentUserInitializer>(provider => provider.GetRequiredService<ICurrentUserService>());
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

using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Recovery.BackOffice.BuildingBlocks.Web.Versioning;

public static class Extensions
{
    public static IServiceCollection AddRecoveryVersioning(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        return services;
    }
}
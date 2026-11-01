using Microsoft.Extensions.DependencyInjection;

namespace Recovery.BackOffice.BuildingBlocks.Web.Sse;

public static class Extensions
{
    /// <summary>
    /// Registers SSE connection manager as a singleton.
    /// </summary>
    public static IServiceCollection AddRecoverySse(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<SseConnectionManager>();
        services.AddScoped<ISseTokenService, SseTokenService>();
        return services;
    }
}

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Recovery.BackOffice.BuildingBlocks.Web.Modules;

public static class ModuleLoader
{
    public static IHostApplicationBuilder AddModules(
        this IHostApplicationBuilder builder,
        params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assemblies);

        if (assemblies.Length == 0 || assemblies.Any(assembly => assembly is null))
        {
            throw new ArgumentException("Specify at least one non-null assembly.", nameof(assemblies));
        }

        if (builder.Services.Any(service => service.ServiceType == typeof(ModuleRegistry)))
        {
            throw new InvalidOperationException("Modules have already been registered for this host.");
        }

        var registrations = assemblies
            .Distinct()
            .SelectMany(assembly => assembly.GetCustomAttributes<RecoveryModuleAttribute>())
            .OrderBy(registration => registration.Order)
            .ThenBy(registration => registration.ModuleType.FullName, StringComparer.Ordinal)
            .DistinctBy(registration => registration.ModuleType);

        var modules = new List<IModule>();

        foreach (var registration in registrations)
        {
            var module = (IModule)Activator.CreateInstance(registration.ModuleType)!;
            module.ConfigureServices(builder);
            modules.Add(module);
        }

        builder.Services.AddSingleton(new ModuleRegistry(modules.ToArray()));

        return builder;
    }

    public static IApplicationBuilder UseModuleMiddlewares(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        foreach (var module in app.ApplicationServices.GetRequiredService<ModuleRegistry>().Modules)
        {
            module.ConfigureMiddleware(app);
        }

        return app;
    }

    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        foreach (var module in endpoints.ServiceProvider.GetRequiredService<ModuleRegistry>().Modules)
        {
            module.MapEndpoints(endpoints);
        }

        return endpoints;
    }

    private sealed record ModuleRegistry(IReadOnlyList<IModule> Modules);
}

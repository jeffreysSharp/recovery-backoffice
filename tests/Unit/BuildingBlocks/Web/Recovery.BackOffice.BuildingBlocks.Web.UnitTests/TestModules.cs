using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recovery.BackOffice.BuildingBlocks.Web.Modules;
using Recovery.BackOffice.BuildingBlocks.Web.UnitTests;

[assembly: RecoveryModule(typeof(ZetaModule), 20)]
[assembly: RecoveryModule(typeof(BetaModule), 10)]
[assembly: RecoveryModule(typeof(AlphaModule), 10)]
[assembly: RecoveryModule(typeof(AlphaModule), 10)]

namespace Recovery.BackOffice.BuildingBlocks.Web.UnitTests;

public sealed record ModuleRegistration(string Name);

public sealed class LifecycleLog
{
    public List<string> Entries { get; } = [];
}

public abstract class TestModule(string name) : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton(new ModuleRegistration(name));
    }

    public void ConfigureMiddleware(IApplicationBuilder app)
    {
        app.ApplicationServices.GetRequiredService<LifecycleLog>().Entries.Add($"middleware:{name}");
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.ServiceProvider.GetRequiredService<LifecycleLog>().Entries.Add($"endpoints:{name}");
        endpoints.MapGet($"/modules/{name}", () => name);
    }
}

public sealed class AlphaModule() : TestModule("Alpha");

public sealed class BetaModule() : TestModule("Beta");

public sealed class ZetaModule() : TestModule("Zeta");

public sealed class ModuleWithDependency(string dependency) : TestModule(dependency);

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Recovery.BackOffice.BuildingBlocks.Web.Modules;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Web.UnitTests;

public sealed class ModuleLoaderTests
{
    [Fact]
    public async Task AddModules_Should_RegisterEachModuleOnceInOrder_When_AssembliesContainDuplicates()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var assembly = typeof(AlphaModule).Assembly;

        // Act
        builder.AddModules(assembly, assembly);
        await using var app = builder.Build();
        var names = app.Services.GetServices<ModuleRegistration>().Select(module => module.Name);

        // Assert
        Assert.Equal(["Alpha", "Beta", "Zeta"], names);
    }

    [Fact]
    public async Task ModuleLifecycle_Should_ConfigureMiddlewareAndRoutesInOrder_When_HostIsBuilt()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton<LifecycleLog>();
        builder.AddModules(typeof(AlphaModule).Assembly);
        await using var app = builder.Build();

        // Act
        app.UseModuleMiddlewares();
        app.MapModules();

        // Assert
        Assert.Equal(
            ["middleware:Alpha", "middleware:Beta", "middleware:Zeta",
             "endpoints:Alpha", "endpoints:Beta", "endpoints:Zeta"],
            app.Services.GetRequiredService<LifecycleLog>().Entries);
        Assert.Equal(3, ((IEndpointRouteBuilder)app).DataSources.SelectMany(source => source.Endpoints).Count());
    }

    [Fact]
    public async Task AddModules_Should_KeepRegistrationsIndependent_When_TwoHostsAreCreated()
    {
        // Arrange
        var firstBuilder = WebApplication.CreateBuilder();
        var secondBuilder = WebApplication.CreateBuilder();

        // Act
        firstBuilder.AddModules(typeof(AlphaModule).Assembly);
        secondBuilder.AddModules(typeof(AlphaModule).Assembly);
        await using var firstApp = firstBuilder.Build();
        await using var secondApp = secondBuilder.Build();

        // Assert
        Assert.Equal(3, firstApp.Services.GetServices<ModuleRegistration>().Count());
        Assert.Equal(3, secondApp.Services.GetServices<ModuleRegistration>().Count());
    }

    [Fact]
    public void AddModules_Should_RejectDuplicateCall_When_HostAlreadyHasModules()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddModules(typeof(AlphaModule).Assembly);

        Assert.Throws<InvalidOperationException>(
            () => builder.AddModules(typeof(AlphaModule).Assembly));
    }

    [Fact]
    public void AddModules_Should_RejectMissingAssemblies_When_NoAssemblyIsSpecified()
    {
        var builder = WebApplication.CreateBuilder();

        var exception = Assert.Throws<ArgumentException>(() => builder.AddModules());

        Assert.Equal("assemblies", exception.ParamName);
    }

    [Fact]
    public async Task AddModules_Should_LoadNoModules_When_AssemblyHasNoRegistrationAttributes()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddModules(typeof(string).Assembly);
        await using var app = builder.Build();

        Assert.Empty(app.Services.GetServices<ModuleRegistration>());
    }
}

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.Identity.Contracts.Services;
using Xunit;

namespace Recovery.BackOffice.Identity.UnitTests;

public sealed class RequestContextServiceTests
{
    [Fact]
    public void RequestContext_Should_ReadRequestMetadata_When_HttpContextExists()
    {
        // Arrange
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse("192.0.2.10");
        context.Request.Headers.UserAgent = "RecoveryTests/1.0";
        context.Request.Headers["X-Client-Id"] = "backoffice";
        scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = context;

        // Act
        var request = scope.ServiceProvider.GetRequiredService<IRequestContext>();

        // Assert
        Assert.Equal("192.0.2.10", request.IpAddress);
        Assert.Equal("RecoveryTests/1.0", request.UserAgent);
        Assert.Equal("backoffice", request.ClientId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ClientId_Should_DefaultToWeb_When_HeaderIsEmpty(string header)
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Client-Id"] = header;
        scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = context;

        Assert.Equal("web", scope.ServiceProvider.GetRequiredService<IRequestContext>().ClientId);
    }

    [Fact]
    public void RequestContext_Should_ReturnSafeDefaults_When_NoHttpContextExists()
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = null;

        var request = scope.ServiceProvider.GetRequiredService<IRequestContext>();

        Assert.Null(request.IpAddress);
        Assert.Null(request.UserAgent);
        Assert.Null(request.Origin);
        Assert.Equal("web", request.ClientId);
    }

    [Fact]
    public void Origin_Should_PreferConfiguredOrigin_When_RequestHasAnotherHost()
    {
        using var host = CreateHost("https://api.example.test/recovery/");
        using var scope = host.Services.CreateScope();
        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("another.example.test");
        scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = context;

        Assert.Equal("https://api.example.test/recovery",
            scope.ServiceProvider.GetRequiredService<IRequestContext>().Origin);
    }

    [Fact]
    public void Origin_Should_UseSchemeHostAndPathBase_When_ConfiguredOriginIsMissing()
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api.example.test", 8443);
        context.Request.PathBase = "/recovery/";
        scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = context;

        Assert.Equal("https://api.example.test:8443/recovery",
            scope.ServiceProvider.GetRequiredService<IRequestContext>().Origin);
    }

    [Fact]
    public void Origin_Should_ReturnConfiguredOrigin_When_HttpContextIsMissing()
    {
        using var host = CreateHost("https://api.example.test/");
        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = null;

        Assert.Equal("https://api.example.test",
            scope.ServiceProvider.GetRequiredService<IRequestContext>().Origin);
    }

    [Fact]
    public void RequestContext_Should_ShareAliasesAndSeparateScopes_When_ResolvedFromContainer()
    {
        using var host = CreateHost();
        using var firstScope = host.Services.CreateScope();
        using var secondScope = host.Services.CreateScope();

        var first = firstScope.ServiceProvider.GetRequiredService<IRequestContext>();

        Assert.Same(first, firstScope.ServiceProvider.GetRequiredService<IRequestContextService>());
        Assert.NotSame(first, secondScope.ServiceProvider.GetRequiredService<IRequestContext>());
    }

    private static IHost CreateHost(string? origin = null)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["OriginOptions:OriginUrl"] = origin
        });
        new IdentityModule().ConfigureServices(builder);

        return builder.Build();
    }
}

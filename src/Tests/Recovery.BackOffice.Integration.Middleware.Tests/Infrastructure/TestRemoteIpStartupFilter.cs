using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Recovery.BackOffice.Integration.Middleware.Tests.Infrastructure;

/// <summary>Assigns isolated client addresses inside TestServer only.</summary>
public sealed class TestRemoteIpStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
    {
        app.Use(async (context, nextMiddleware) =>
        {
            if (IPAddress.TryParse(context.Request.Headers["X-Test-Remote-Ip"].FirstOrDefault(), out var address))
            {
                context.Connection.RemoteIpAddress = address;
            }

            await nextMiddleware();
        });
        next(app);
    };
}

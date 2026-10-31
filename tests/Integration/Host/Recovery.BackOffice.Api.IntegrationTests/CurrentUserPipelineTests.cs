using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Xunit;

namespace Recovery.BackOffice.Api.IntegrationTests;

public sealed class CurrentUserPipelineTests
{
    [Fact]
    public async Task RequestPipeline_Should_InitializeIndependentUsers_When_RequestsHaveDifferentPrincipals()
    {
        // Arrange
        var observations = new ConcurrentQueue<UserObservation>();
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");
            builder.ConfigureServices(services =>
                services.AddSingleton<IStartupFilter>(new TestPrincipalFilter(observations)));
        });
        using var client = factory.CreateClient();
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        // Act
        foreach (var userId in new[] { firstId, secondId, Guid.Empty })
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/");
            if (userId != Guid.Empty)
            {
                request.Headers.Add("X-Test-User-Id", userId.ToString());
            }

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Assert.Equal("Hello World!", await response.Content.ReadAsStringAsync());
        }

        // Assert
        var users = observations.ToArray();
        Assert.Equal(3, users.Length);
        Assert.Equal([firstId, secondId, Guid.Empty], users.Select(user => user.UserId));
        Assert.Equal([true, true, false], users.Select(user => user.Authenticated));
        Assert.NotSame(users[0].Context, users[1].Context);
        Assert.NotSame(users[1].Context, users[2].Context);
    }

    private sealed record UserObservation(ICurrentUser Context, Guid UserId, bool Authenticated);

    private sealed class TestPrincipalFilter(ConcurrentQueue<UserObservation> observations) : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            // Synthetic principals exist only in this test host; no production header authentication.
            app.Use(async (context, nextMiddleware) =>
            {
                var userId = context.Request.Headers["X-Test-User-Id"].ToString();
                if (!string.IsNullOrEmpty(userId))
                {
                    context.User = new ClaimsPrincipal(new ClaimsIdentity(
                        [new Claim(ClaimTypes.NameIdentifier, userId)], "TestAuthentication"));
                }

                await nextMiddleware(context);

                var user = context.RequestServices.GetRequiredService<ICurrentUser>();
                observations.Enqueue(new UserObservation(user, user.GetUserId(), user.IsAuthenticated()));
            });

            next(app);
        };
    }
}

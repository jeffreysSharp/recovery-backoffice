using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.BuildingBlocks.Shared.Constants;
using Recovery.BackOffice.BuildingBlocks.Web.Auth;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Web.UnitTests;

public sealed class CurrentUserMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Should_InitializeUserBeforeNext_When_RequestIsProcessed()
    {
        // Arrange
        var initializer = new RecordingInitializer();
        var middleware = new CurrentUserMiddleware(initializer);
        var context = new DefaultHttpContext();
        var nextCalls = 0;

        // Act
        await middleware.InvokeAsync(context, receivedContext =>
        {
            Assert.Same(context, receivedContext);
            Assert.Same(context.User, initializer.User);
            nextCalls++;
            return Task.CompletedTask;
        });

        // Assert
        Assert.Equal(1, nextCalls);
        Assert.Equal(1, initializer.Initializations);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task InvokeAsync_Should_TagActivityOnlyForAuthenticatedUsers_When_ActivityExists(bool authenticated)
    {
        var middleware = new CurrentUserMiddleware(new RecordingInitializer());
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, "user-id"),
                 new Claim(CustomClaims.Tenant, "tenant-a")], authenticated ? "TestAuthentication" : null)),
            TraceIdentifier = "trace-id"
        };
        using var activity = new Activity("MiddlewareTest").Start();

        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        Assert.Equal(authenticated ? "user-id" : null, activity.GetTagItem("recovery.user_id"));
        Assert.Equal(authenticated ? "tenant-a" : null, activity.GetTagItem("recovery.tenant_id"));
        Assert.Equal(authenticated ? "trace-id" : null, activity.GetTagItem("recovery.correlation_id"));
    }

    [Fact]
    public async Task InvokeAsync_Should_PropagateException_When_NextComponentFails()
    {
        var initializer = new RecordingInitializer();
        var middleware = new CurrentUserMiddleware(initializer);
        var failure = new InvalidOperationException("Next component failed.");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => middleware.InvokeAsync(new DefaultHttpContext(), _ => throw failure));

        Assert.Same(failure, exception);
        Assert.Equal(1, initializer.Initializations);
    }

    [Fact]
    public async Task InvokeAsync_Should_RejectMissingContext_When_ContextIsNull()
    {
        var middleware = new CurrentUserMiddleware(new RecordingInitializer());

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => middleware.InvokeAsync(null!, _ => Task.CompletedTask));
    }

    [Fact]
    public async Task InvokeAsync_Should_RejectMissingNext_When_DelegateIsNull()
    {
        var initializer = new RecordingInitializer();
        var middleware = new CurrentUserMiddleware(initializer);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => middleware.InvokeAsync(new DefaultHttpContext(), null!));
        Assert.Equal(0, initializer.Initializations);
    }

    private sealed class RecordingInitializer : ICurrentUserInitializer
    {
        public ClaimsPrincipal? User { get; private set; }

        public int Initializations { get; private set; }

        public void SetCurrentUser(ClaimsPrincipal user)
        {
            User = user;
            Initializations++;
        }

        public void SetCurrentUserId(string userId) => throw new NotSupportedException();
    }
}

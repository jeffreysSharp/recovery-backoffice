using Recovery.BackOffice.BuildingBlocks.Eventing.Inbox;
using Recovery.BackOffice.Integration.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Recovery.BackOffice.Integration.Tests.Tests.Eventing;

[Collection(RecoveryCollectionDefinition.Name)]
public sealed class InboxHandlerIdentityTests(RecoveryWebApplicationFactory factory)
{
    [Fact]
    public async Task Long_Generic_Handler_Name_Is_Persisted_And_Remains_Idempotent()
    {
        using var scope = factory.Services.CreateScope();
        var inbox = scope.ServiceProvider.GetRequiredService<IInboxStore>();
        var eventId = Guid.NewGuid();
        var handler = "Recovery.BackOffice.Handler<" + new string('a', 400) + ">";
        await inbox.MarkProcessedAsync(eventId, handler, "root", "TestEvent");
        (await inbox.HasProcessedAsync(eventId, handler)).ShouldBeTrue();
        await inbox.MarkProcessedAsync(eventId, handler, "root", "TestEvent");
        (await inbox.HasProcessedAsync(eventId, handler)).ShouldBeTrue();
    }

    [Fact]
    public async Task Handlers_Sharing_A_Long_Prefix_Are_Distinct_Consumers()
    {
        using var scope = factory.Services.CreateScope();
        var inbox = scope.ServiceProvider.GetRequiredService<IInboxStore>();
        var eventId = Guid.NewGuid();
        var prefix = new string('a', 400);
        await inbox.MarkProcessedAsync(eventId, prefix + "First", "root", "TestEvent");
        (await inbox.HasProcessedAsync(eventId, prefix + "Second")).ShouldBeFalse();
        await inbox.MarkProcessedAsync(eventId, prefix + "Second", "root", "TestEvent");
        (await inbox.HasProcessedAsync(eventId, prefix + "First")).ShouldBeTrue();
        (await inbox.HasProcessedAsync(eventId, prefix + "Second")).ShouldBeTrue();
    }
}

using Recovery.BackOffice.BuildingBlocks.Core.Domain;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Core.UnitTests;

public sealed class DomainEventTests
{
    [Fact]
    public void Create_Should_PassGeneratedIdentityAndUtcTimestamp_When_FactoryIsProvided()
    {
        // Arrange
        var capturedId = Guid.Empty;
        var capturedTimestamp = default(DateTimeOffset);

        // Act
        var domainEvent = DomainEvent.Create((id, timestamp) =>
        {
            capturedId = id;
            capturedTimestamp = timestamp;
            return new TestEvent(id, timestamp);
        });

        // Assert
        Assert.NotEqual(Guid.Empty, capturedId);
        Assert.NotEqual(default, capturedTimestamp);
        Assert.Equal(TimeSpan.Zero, capturedTimestamp.Offset);
        Assert.Equal(capturedId, domainEvent.EventId);
        Assert.Equal(capturedTimestamp, domainEvent.OccurredOnUtc);
    }

    [Fact]
    public void Create_Should_GenerateDifferentIdentities_When_CalledTwice()
    {
        var first = DomainEvent.Create((id, timestamp) => new TestEvent(id, timestamp));
        var second = DomainEvent.Create((id, timestamp) => new TestEvent(id, timestamp));

        Assert.NotEqual(first.EventId, second.EventId);
    }

    [Fact]
    public void Create_Should_RejectFactory_When_FactoryIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => DomainEvent.Create<TestEvent>(null!));

        Assert.Equal("factory", exception.ParamName);
    }

    [Fact]
    public void Create_Should_PropagateFailure_When_FactoryThrows()
    {
        var failure = new InvalidOperationException("Factory failed.");

        var exception = Assert.Throws<InvalidOperationException>(
            () => DomainEvent.Create<TestEvent>((_, _) => throw failure));

        Assert.Same(failure, exception);
    }

    private sealed record TestEvent(Guid Id, DateTimeOffset Timestamp) : DomainEvent(Id, Timestamp);
}

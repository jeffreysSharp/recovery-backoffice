using Recovery.BackOffice.BuildingBlocks.Core.Domain;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Core.UnitTests;

public sealed class BaseEntityTests
{
    [Fact]
    public void AddDomainEvent_Should_PreserveEventOrder_When_SeveralEventsAreRaised()
    {
        // Arrange
        var entity = new TestAggregate(Guid.NewGuid());
        var first = new TestEvent(Guid.NewGuid());
        var second = new TestEvent(Guid.NewGuid());

        // Act
        entity.Raise(first);
        entity.Raise(second);

        // Assert
        Assert.Equal<IDomainEvent>([first, second], entity.DomainEvents);
    }

    [Fact]
    public void ClearDomainEvents_Should_RemoveEventsAndPreserveIdentity_When_EntityHasPendingEvents()
    {
        var id = Guid.NewGuid();
        var entity = new TestAggregate(id);
        entity.Raise(new TestEvent(Guid.NewGuid()));

        entity.ClearDomainEvents();

        Assert.Empty(entity.DomainEvents);
        Assert.Equal(id, entity.Id);
    }

    [Fact]
    public void ClearDomainEvents_Should_AllowNewEvents_When_PreviousEventsWereCleared()
    {
        var entity = new TestAggregate(Guid.NewGuid());
        entity.Raise(new TestEvent(Guid.NewGuid()));
        entity.ClearDomainEvents();
        entity.ClearDomainEvents();
        var next = new TestEvent(Guid.NewGuid());

        entity.Raise(next);

        Assert.Same(next, Assert.Single(entity.DomainEvents));
    }

    [Fact]
    public void AddDomainEvent_Should_KeepEventsIndependent_When_EntitiesAreDifferent()
    {
        var first = new TestAggregate(Guid.NewGuid());
        var second = new TestAggregate(Guid.NewGuid());

        first.Raise(new TestEvent(Guid.NewGuid()));

        Assert.Single(first.DomainEvents);
        Assert.Empty(second.DomainEvents);
    }

    private sealed class TestAggregate : AggregateRoot<Guid>
    {
        public TestAggregate(Guid id) => Id = id;

        public void Raise(IDomainEvent domainEvent) => AddDomainEvent(domainEvent);
    }

    private sealed record TestEvent(Guid Id)
        : DomainEvent(Id, new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
}

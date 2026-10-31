using Recovery.BackOffice.BuildingBlocks.Core.Domain;

namespace Recovery.BackOffice.Modules.Chat.Domain.Events;

public sealed record MessageDeletedDomainEvent(
    Guid ChannelId,
    Guid MessageId,
    string AuthorUserId,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);

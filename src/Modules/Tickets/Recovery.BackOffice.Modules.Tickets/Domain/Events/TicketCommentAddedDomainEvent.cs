using Recovery.BackOffice.BuildingBlocks.Core.Domain;

namespace Recovery.BackOffice.Modules.Tickets.Domain.Events;

public sealed record TicketCommentAddedDomainEvent(
    Guid TicketId,
    Guid CommentId,
    Guid AuthorUserId,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);

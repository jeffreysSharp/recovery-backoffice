using Recovery.BackOffice.BuildingBlocks.Core.Domain;
using Recovery.BackOffice.Modules.Tickets.Contracts.Dtos;

namespace Recovery.BackOffice.Modules.Tickets.Domain.Events;

public sealed record TicketStatusChangedDomainEvent(
    Guid TicketId,
    TicketStatus PreviousStatus,
    TicketStatus NewStatus,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);

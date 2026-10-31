using Recovery.BackOffice.BuildingBlocks.Core.Domain;
using Recovery.BackOffice.Modules.Tickets.Contracts.Dtos;

namespace Recovery.BackOffice.Modules.Tickets.Domain.Events;

public sealed record TicketCreatedDomainEvent(
    Guid TicketId,
    string Number,
    string Title,
    TicketPriority Priority,
    Guid ReporterUserId,
    Guid? AssignedToUserId,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);

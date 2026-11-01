using Recovery.BackOffice.BuildingBlocks.Core.Domain;
using Recovery.BackOffice.Modules.Chat.Contracts.v1.DTOs;

namespace Recovery.BackOffice.Modules.Chat.Domain.Events;

public sealed record ChannelCreatedDomainEvent(
    Guid ChannelId,
    ChannelType Type,
    string? Name,
    string CreatedByUserId,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);

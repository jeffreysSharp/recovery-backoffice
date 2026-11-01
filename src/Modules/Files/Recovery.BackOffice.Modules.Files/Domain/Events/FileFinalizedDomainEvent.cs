using Recovery.BackOffice.BuildingBlocks.Core.Domain;
using Recovery.BackOffice.Modules.Files.Contracts.v1.DTOs;

namespace Recovery.BackOffice.Modules.Files.Domain.Events;

public sealed record FileFinalizedDomainEvent(
    Guid FileAssetId,
    string OwnerType,
    Guid? OwnerId,
    FileAssetStatus FinalStatus,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);

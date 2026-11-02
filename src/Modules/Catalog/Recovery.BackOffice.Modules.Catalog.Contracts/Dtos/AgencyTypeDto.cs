namespace Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
public sealed record AgencyTypeDto(Guid Id, string Name, string? Description, bool IsActive);

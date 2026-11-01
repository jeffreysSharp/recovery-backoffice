namespace Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;

public sealed record StateDto(Guid Id, Guid CountryId, string CountryName, string Name, string Code, string? IbgeCode, bool IsActive);

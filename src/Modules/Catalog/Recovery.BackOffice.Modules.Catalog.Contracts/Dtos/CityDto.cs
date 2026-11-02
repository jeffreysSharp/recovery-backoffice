namespace Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
public sealed record CityDto(Guid Id, Guid StateId, string StateName, Guid CountryId, string CountryName, string Name, string? IbgeCode, bool IsActive);

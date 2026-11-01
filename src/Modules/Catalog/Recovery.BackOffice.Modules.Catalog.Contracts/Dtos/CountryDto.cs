namespace Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;

public sealed record CountryDto(Guid Id, string Name, string Iso2Code, string Iso3Code,
    string? NumericCode, string? PhoneCode, bool IsActive);

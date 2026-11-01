using Mediator;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;

public sealed record CreateCountryCommand(string Name, string Iso2Code, string Iso3Code, string? NumericCode = null, string? PhoneCode = null, bool IsActive = true) : ICommand<Guid>;

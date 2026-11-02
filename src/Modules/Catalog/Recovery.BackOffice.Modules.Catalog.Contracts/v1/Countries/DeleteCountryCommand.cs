using Mediator;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;

public sealed record DeleteCountryCommand(Guid CountryId) : ICommand<Unit>;

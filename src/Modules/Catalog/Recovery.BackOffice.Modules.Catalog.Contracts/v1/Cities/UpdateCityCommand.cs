using Mediator;
namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;
public sealed record UpdateCityCommand(Guid CityId, Guid StateId, string Name, string? IbgeCode = null, bool IsActive = true) : ICommand<Guid>;

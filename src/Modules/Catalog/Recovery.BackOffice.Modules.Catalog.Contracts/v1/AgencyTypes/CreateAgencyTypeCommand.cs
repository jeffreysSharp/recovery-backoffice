using Mediator;
namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
public sealed record CreateAgencyTypeCommand(string Name, string? Description = null, bool IsActive = true) : ICommand<Guid>;

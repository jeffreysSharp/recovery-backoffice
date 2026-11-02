using Mediator;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;

public sealed record DeleteAgencyTypeCommand(Guid AgencyTypeId) : ICommand<Unit>;

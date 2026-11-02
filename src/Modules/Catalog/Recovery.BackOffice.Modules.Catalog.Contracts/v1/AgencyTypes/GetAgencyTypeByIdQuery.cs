using Mediator;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;

public sealed record GetAgencyTypeByIdQuery(Guid AgencyTypeId) : IQuery<AgencyTypeDto>;

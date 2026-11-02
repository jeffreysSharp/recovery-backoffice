using Mediator;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;

public sealed record GetCityByIdQuery(Guid CityId) : IQuery<CityDto>;

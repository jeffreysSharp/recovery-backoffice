using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Brands;

public sealed record GetBrandByIdQuery(Guid BrandId) : IQuery<BrandDto>;

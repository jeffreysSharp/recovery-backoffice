using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Products;

public sealed record GetProductByIdQuery(Guid ProductId) : IQuery<ProductDto>;

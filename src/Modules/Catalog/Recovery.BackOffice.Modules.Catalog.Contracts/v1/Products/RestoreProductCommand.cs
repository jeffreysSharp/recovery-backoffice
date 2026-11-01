using Mediator;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Products;

public sealed record RestoreProductCommand(Guid ProductId) : ICommand<Guid>;

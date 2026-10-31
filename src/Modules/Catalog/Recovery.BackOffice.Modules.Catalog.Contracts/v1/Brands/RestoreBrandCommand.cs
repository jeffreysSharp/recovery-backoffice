using Mediator;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Brands;

public sealed record RestoreBrandCommand(Guid BrandId) : ICommand<Guid>;

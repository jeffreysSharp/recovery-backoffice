using Mediator;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Categories;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string? Description = null,
    Guid? ParentCategoryId = null) : ICommand<Guid>;

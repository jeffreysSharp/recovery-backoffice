using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Catalog.Contracts.v1.Categories;

public sealed record GetCategoryTreeQuery : IQuery<IReadOnlyList<CategoryTreeNodeDto>>;

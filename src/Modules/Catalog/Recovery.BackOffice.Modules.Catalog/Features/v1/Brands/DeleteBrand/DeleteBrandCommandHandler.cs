using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Brands;
using Recovery.BackOffice.Modules.Catalog.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Brands.DeleteBrand;

public sealed class DeleteBrandCommandHandler(CatalogDbContext dbContext)
    : ICommandHandler<DeleteBrandCommand, Unit>
{
    public async ValueTask<Unit> Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var brand = await dbContext.Brands
            .FirstOrDefaultAsync(b => b.Id == command.BrandId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Brand {command.BrandId} not found.");

        dbContext.Brands.Remove(brand);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}

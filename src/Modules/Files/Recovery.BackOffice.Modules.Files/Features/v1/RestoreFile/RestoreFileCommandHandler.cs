using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Files.Contracts.v1.Commands;
using Recovery.BackOffice.Modules.Files.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Files.Features.v1.RestoreFile;

public sealed class RestoreFileCommandHandler(FilesDbContext db)
    : ICommandHandler<RestoreFileCommand, Unit>
{
    public async ValueTask<Unit> Handle(RestoreFileCommand cmd, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cmd);

        // IgnoreQueryFilters because the SoftDelete filter would otherwise hide the row.
        var f = await db.FileAssets
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == cmd.FileAssetId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("file not found");

        if (!f.IsDeleted)
        {
            return Unit.Value; // idempotent — already live
        }

        f.Restore();
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}

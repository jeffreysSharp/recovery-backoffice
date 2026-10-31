using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.BuildingBlocks.Storage.Services;
using Recovery.BackOffice.Modules.Files.Contracts;
using Recovery.BackOffice.Modules.Files.Contracts.v1.DTOs;
using Recovery.BackOffice.Modules.Files.Contracts.v1.Queries;
using Recovery.BackOffice.Modules.Files.Data;
using Recovery.BackOffice.Modules.Files.Services;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Recovery.BackOffice.Modules.Files.Features.v1.GetFileDownloadUrl;

public sealed class GetFileDownloadUrlQueryHandler(
    FilesDbContext db,
    IStorageService storage,
    FileAccessPolicyRegistry policies,
    ICurrentUser currentUser,
    IOptions<FilesOptions> options)
    : IQueryHandler<GetFileDownloadUrlQuery, PresignedDownloadResponse>
{
    public async ValueTask<PresignedDownloadResponse> Handle(GetFileDownloadUrlQuery q, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(q);

        var f = await db.FileAssets.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == q.FileAssetId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("file not found");

        var userId = currentUser.GetUserId().ToString();
        var policy = policies.Resolve(f.OwnerType)
            ?? throw new NotFoundException("file not found");

        var ctx = new FileAccessContext(f.Id, f.OwnerType, f.OwnerId, f.CreatedByUserId, (int)f.Visibility);
        if (!await policy.CanReadAsync(ctx, userId, cancellationToken).ConfigureAwait(false))
        {
            throw new NotFoundException("file not found");
        }

        var ttl = TimeSpan.FromMinutes(options.Value.DownloadUrlTtlMinutes);
        var mode = q.Inline ? "inline" : "attachment";
        var disposition = $"{mode}; filename=\"{f.OriginalFileName}\"";
        var url = await storage.GenerateDownloadUrlAsync(f.StorageKey, ttl, disposition, cancellationToken).ConfigureAwait(false);
        return new PresignedDownloadResponse(url, DateTimeOffset.UtcNow.Add(ttl));
    }
}

using Recovery.BackOffice.Modules.Files.Contracts.v1.DTOs;
using Recovery.BackOffice.Modules.Files.Domain;

namespace Recovery.BackOffice.Modules.Files.Features.v1.Internal;

/// <summary>
/// Shared mapper from FileAsset → FileAssetDto so handlers don't duplicate the projection.
/// </summary>
internal static class FileAssetMapper
{
    public static FileAssetDto ToDto(FileAsset f, string? publicUrl = null) =>
        new(f.Id, f.OwnerType, f.OwnerId, f.OriginalFileName, f.ContentType, f.SizeBytes,
            f.Visibility, f.Status, (int)f.ScanStatus, f.CreatedAtUtc, publicUrl,
            f.CreatedByUserId, f.DeletedOnUtc, f.DeletedBy);
}

using FluentValidation;
using Recovery.BackOffice.BuildingBlocks.Core.Localization;
using Recovery.BackOffice.BuildingBlocks.Shared.Storage;
using Recovery.BackOffice.BuildingBlocks.Storage;
using Microsoft.Extensions.Localization;

namespace Recovery.BackOffice.Modules.Identity.Features.v1.Users;

public sealed class UserImageValidator : AbstractValidator<FileUploadRequest>
{
    public UserImageValidator(IStringLocalizer<SharedResources> localizer) : this(FileType.Image, localizer) { }
    public UserImageValidator(FileType fileType, IStringLocalizer<SharedResources> localizer)
    {
        var rules = FileTypeMetadata.GetRules(fileType);

        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(file => rules.AllowedExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            .WithMessage(_ => localizer["Validation.AllowedExtensions", string.Join(", ", rules.AllowedExtensions)]);

        RuleFor(x => x.Data)
            .NotEmpty()
            .Must(data => data.Count <= rules.MaxSizeInMB * 1024 * 1024)
            .WithMessage(_ => localizer["Validation.MaxFileSize", rules.MaxSizeInMB]);
    }
}
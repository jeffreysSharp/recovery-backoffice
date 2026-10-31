using FluentValidation;
using Recovery.BackOffice.Modules.Files.Contracts.v1.Commands;

namespace Recovery.BackOffice.Modules.Files.Features.v1.FinalizeUpload;

public sealed class FinalizeUploadCommandValidator : AbstractValidator<FinalizeUploadCommand>
{
    public FinalizeUploadCommandValidator()
    {
        RuleFor(x => x.FileAssetId).NotEmpty();
    }
}

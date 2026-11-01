using FluentValidation;
using Recovery.BackOffice.Modules.Files.Contracts.v1.Commands;

namespace Recovery.BackOffice.Modules.Files.Features.v1.RestoreFile;

public sealed class RestoreFileCommandValidator : AbstractValidator<RestoreFileCommand>
{
    public RestoreFileCommandValidator()
    {
        RuleFor(x => x.FileAssetId).NotEmpty();
    }
}

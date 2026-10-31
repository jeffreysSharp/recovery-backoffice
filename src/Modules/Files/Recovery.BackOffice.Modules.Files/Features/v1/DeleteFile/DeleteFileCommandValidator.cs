using FluentValidation;
using Recovery.BackOffice.Modules.Files.Contracts.v1.Commands;

namespace Recovery.BackOffice.Modules.Files.Features.v1.DeleteFile;

public sealed class DeleteFileCommandValidator : AbstractValidator<DeleteFileCommand>
{
    public DeleteFileCommandValidator()
    {
        RuleFor(x => x.FileAssetId).NotEmpty();
    }
}

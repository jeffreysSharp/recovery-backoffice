using Mediator;

namespace Recovery.BackOffice.Modules.Files.Contracts.v1.Commands;

public sealed record DeleteFileCommand(Guid FileAssetId) : ICommand<Unit>;

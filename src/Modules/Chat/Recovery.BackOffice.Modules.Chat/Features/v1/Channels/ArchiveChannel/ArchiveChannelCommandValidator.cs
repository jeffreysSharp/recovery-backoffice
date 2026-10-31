using FluentValidation;
using Recovery.BackOffice.Modules.Chat.Contracts.v1.Commands;

namespace Recovery.BackOffice.Modules.Chat.Features.v1.Channels.ArchiveChannel;

public sealed class ArchiveChannelCommandValidator : AbstractValidator<ArchiveChannelCommand>
{
    public ArchiveChannelCommandValidator()
    {
        RuleFor(x => x.ChannelId).NotEmpty();
    }
}

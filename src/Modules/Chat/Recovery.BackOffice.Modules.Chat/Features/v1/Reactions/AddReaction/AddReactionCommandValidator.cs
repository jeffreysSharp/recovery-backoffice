using FluentValidation;
using Recovery.BackOffice.Modules.Chat.Contracts.v1.Commands;

namespace Recovery.BackOffice.Modules.Chat.Features.v1.Reactions.AddReaction;

public sealed class AddReactionCommandValidator : AbstractValidator<AddReactionCommand>
{
    public AddReactionCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.Emoji).NotEmpty().MaximumLength(64);
    }
}

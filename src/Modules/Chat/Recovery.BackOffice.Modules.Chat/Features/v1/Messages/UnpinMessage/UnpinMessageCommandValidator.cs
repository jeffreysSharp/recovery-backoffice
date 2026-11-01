using FluentValidation;
using Recovery.BackOffice.Modules.Chat.Contracts.v1.Commands;

namespace Recovery.BackOffice.Modules.Chat.Features.v1.Messages.UnpinMessage;

public sealed class UnpinMessageCommandValidator : AbstractValidator<UnpinMessageCommand>
{
    public UnpinMessageCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
    }
}

using FluentValidation;
using Recovery.BackOffice.Modules.Chat.Contracts.v1.Commands;

namespace Recovery.BackOffice.Modules.Chat.Features.v1.Messages.DeleteMessage;

public sealed class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
    }
}

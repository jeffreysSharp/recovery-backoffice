using FluentValidation;
using Recovery.BackOffice.Modules.Notifications.Contracts.v1.Commands;

namespace Recovery.BackOffice.Modules.Notifications.Features.v1.MarkNotificationRead;

public sealed class MarkNotificationReadCommandValidator : AbstractValidator<MarkNotificationReadCommand>
{
    public MarkNotificationReadCommandValidator()
    {
        RuleFor(x => x.NotificationId).NotEmpty();
    }
}

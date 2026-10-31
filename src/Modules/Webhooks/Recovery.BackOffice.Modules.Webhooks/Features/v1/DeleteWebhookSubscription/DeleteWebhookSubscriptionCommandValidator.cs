using FluentValidation;
using Recovery.BackOffice.Modules.Webhooks.Contracts.v1.DeleteWebhookSubscription;

namespace Recovery.BackOffice.Modules.Webhooks.Features.v1.DeleteWebhookSubscription;

public sealed class DeleteWebhookSubscriptionCommandValidator : AbstractValidator<DeleteWebhookSubscriptionCommand>
{
    public DeleteWebhookSubscriptionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

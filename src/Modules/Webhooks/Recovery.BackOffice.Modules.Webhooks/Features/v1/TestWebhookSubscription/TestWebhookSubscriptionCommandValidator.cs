using FluentValidation;
using Recovery.BackOffice.Modules.Webhooks.Contracts.v1.TestWebhookSubscription;

namespace Recovery.BackOffice.Modules.Webhooks.Features.v1.TestWebhookSubscription;

public sealed class TestWebhookSubscriptionCommandValidator : AbstractValidator<TestWebhookSubscriptionCommand>
{
    public TestWebhookSubscriptionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

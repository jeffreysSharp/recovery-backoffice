using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Webhooks.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Webhooks.Contracts.v1.GetWebhookSubscriptions;

public sealed record GetWebhookSubscriptionsQuery(int PageNumber = 1, int PageSize = 10)
    : IQuery<PagedResponse<WebhookSubscriptionDto>>;

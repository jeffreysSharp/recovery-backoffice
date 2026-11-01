using Recovery.BackOffice.BuildingBlocks.Shared.Quota;
using Mediator;

namespace Recovery.BackOffice.Modules.Billing.Contracts.v1.Plans;

public sealed record UpdatePlanCommand(
    Guid PlanId,
    string Name,
    decimal MonthlyBasePrice,
    IReadOnlyDictionary<QuotaResource, decimal>? OverageRates = null,
    PlanInterval Interval = PlanInterval.Monthly,
    decimal? AnnualPrice = null) : ICommand<Guid>;

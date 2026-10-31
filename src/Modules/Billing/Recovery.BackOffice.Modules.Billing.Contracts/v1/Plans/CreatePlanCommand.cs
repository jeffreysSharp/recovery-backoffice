using Recovery.BackOffice.BuildingBlocks.Shared.Quota;
using Mediator;

namespace Recovery.BackOffice.Modules.Billing.Contracts.v1.Plans;

public sealed record CreatePlanCommand(
    string Key,
    string Name,
    string Currency,
    decimal MonthlyBasePrice,
    IReadOnlyDictionary<QuotaResource, decimal>? OverageRates = null,
    PlanInterval Interval = PlanInterval.Monthly,
    decimal? AnnualPrice = null) : ICommand<Guid>;

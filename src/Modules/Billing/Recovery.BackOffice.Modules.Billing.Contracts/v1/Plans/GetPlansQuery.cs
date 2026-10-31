using Recovery.BackOffice.Modules.Billing.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Billing.Contracts.v1.Plans;

public sealed record GetPlansQuery(bool IncludeInactive = false) : IQuery<IReadOnlyList<BillingPlanDto>>;

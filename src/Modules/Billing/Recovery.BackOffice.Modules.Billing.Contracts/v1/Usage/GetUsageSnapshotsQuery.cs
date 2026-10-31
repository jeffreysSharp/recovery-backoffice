using Recovery.BackOffice.Modules.Billing.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Billing.Contracts.v1.Usage;

public sealed record GetUsageSnapshotsQuery(
    string? TenantId = null,
    int? PeriodYear = null,
    int? PeriodMonth = null) : IQuery<IReadOnlyList<UsageSnapshotDto>>;

using Recovery.BackOffice.Modules.Auditing.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Auditing.Contracts.v1.GetAuditById;

public sealed record GetAuditByIdQuery(Guid Id) : IQuery<AuditDetailDto>;
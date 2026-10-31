using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Tickets.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Tickets.Contracts.v1.Tickets;

public sealed record ListTrashedTicketsQuery(int PageNumber = 1, int PageSize = 20)
    : IQuery<PagedResponse<TicketDto>>;

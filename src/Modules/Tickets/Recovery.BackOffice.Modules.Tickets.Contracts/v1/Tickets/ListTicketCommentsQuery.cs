using Recovery.BackOffice.Modules.Tickets.Contracts.Dtos;
using Mediator;

namespace Recovery.BackOffice.Modules.Tickets.Contracts.v1.Tickets;

public sealed record ListTicketCommentsQuery(Guid TicketId) : IQuery<IReadOnlyList<TicketCommentDto>>;

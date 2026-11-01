using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Modules.Tickets.Contracts.Dtos;
using Recovery.BackOffice.Modules.Tickets.Contracts.v1.Tickets;
using Recovery.BackOffice.Modules.Tickets.Data;
using Recovery.BackOffice.Modules.Tickets.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.ListTrashedTickets;

public sealed class ListTrashedTicketsQueryHandler(TicketsDbContext dbContext)
    : IQueryHandler<ListTrashedTicketsQuery, PagedResponse<TicketDto>>
{
    public async ValueTask<PagedResponse<TicketDto>> Handle(
        ListTrashedTicketsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        int page = query.PageNumber < 1 ? 1 : query.PageNumber;
        int size = query.PageSize is < 1 or > 200 ? 20 : query.PageSize;

        var q = dbContext.Tickets
            .AsNoTracking()
            .IgnoreQueryFilters([QueryFilters.SoftDelete])
            .Where(t => t.IsDeleted)
            .OrderByDescending(t => t.DeletedOnUtc);

        long total = await q.LongCountAsync(cancellationToken).ConfigureAwait(false);
        var tickets = await q
            .Skip(PaginationExtensions.GetOffset(page, size))
            .Take(size)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResponse<TicketDto>
        {
            Items = tickets.Select(t => t.ToDto(0)).ToList(),
            PageNumber = page,
            PageSize = size,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)size),
        };
    }
}

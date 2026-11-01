using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Tickets.Contracts.Authorization;
using Recovery.BackOffice.Modules.Tickets.Contracts.v1.Tickets;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.CloseTicket;

public static class CloseTicketEndpoint
{
    internal static RouteHandlerBuilder MapCloseTicketEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/tickets/{ticketId:guid}/close",
                async (Guid ticketId, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(new CloseTicketCommand(ticketId), ct)))
            .WithName("CloseTicket")
            .WithSummary("Close a resolved ticket")
            .RequirePermission(TicketsPermissions.Tickets.Close)
            .WithIdempotency();
    }
}

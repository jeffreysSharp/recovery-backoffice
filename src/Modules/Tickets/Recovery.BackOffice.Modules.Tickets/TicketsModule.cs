using Asp.Versioning;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Constants;
using Recovery.BackOffice.BuildingBlocks.Web.Modules;
using Recovery.BackOffice.Modules.Tickets.Contracts.Authorization;
using Recovery.BackOffice.Modules.Tickets.Data;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.AddTicketComment;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.AssignTicket;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.CloseTicket;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.CreateTicket;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.DeleteTicket;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.GetTicketById;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.ListTicketComments;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.ListTrashedTickets;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.ReopenTicket;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.ResolveTicket;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.RestoreTicket;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.SearchTickets;
using Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.UpdateTicket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: RecoveryModule(typeof(Recovery.BackOffice.Modules.Tickets.TicketsModule), 700)]

namespace Recovery.BackOffice.Modules.Tickets;

public sealed class TicketsModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(TicketsPermissions.All);

        builder.Services.AddRecoveryDbContext<TicketsDbContext>();
        builder.Services.AddScoped<IDbInitializer, TicketsDbInitializer>();

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<TicketsDbContext>(
                name: "db:tickets",
                failureStatus: HealthStatus.Unhealthy);
    }

    public void ConfigureMiddleware(IApplicationBuilder app)
    {
        // No custom middleware needed
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = endpoints
            .MapGroup("api/v{version:apiVersion}")
            .WithTags("Tickets")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        // Trash + comment routes register before the catch-all `{ticketId:guid}` GET so literal
        // segments win — minimal APIs match the first compatible pattern, so order matters.
        group.MapListTrashedTicketsEndpoint();
        group.MapAddTicketCommentEndpoint();
        group.MapListTicketCommentsEndpoint();

        group.MapRestoreTicketEndpoint();
        group.MapAssignTicketEndpoint();
        group.MapResolveTicketEndpoint();
        group.MapReopenTicketEndpoint();
        group.MapCloseTicketEndpoint();

        group.MapCreateTicketEndpoint();
        group.MapSearchTicketsEndpoint();
        group.MapUpdateTicketEndpoint();
        group.MapDeleteTicketEndpoint();
        group.MapGetTicketByIdEndpoint();
    }
}

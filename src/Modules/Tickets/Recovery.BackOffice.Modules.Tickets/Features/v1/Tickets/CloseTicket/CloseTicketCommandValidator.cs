using FluentValidation;
using Recovery.BackOffice.Modules.Tickets.Contracts.v1.Tickets;

namespace Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.CloseTicket;

public sealed class CloseTicketCommandValidator : AbstractValidator<CloseTicketCommand>
{
    public CloseTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
    }
}

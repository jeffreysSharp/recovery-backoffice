using FluentValidation;
using Recovery.BackOffice.Modules.Tickets.Contracts.v1.Tickets;

namespace Recovery.BackOffice.Modules.Tickets.Features.v1.Tickets.DeleteTicket;

public sealed class DeleteTicketCommandValidator : AbstractValidator<DeleteTicketCommand>
{
    public DeleteTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
    }
}

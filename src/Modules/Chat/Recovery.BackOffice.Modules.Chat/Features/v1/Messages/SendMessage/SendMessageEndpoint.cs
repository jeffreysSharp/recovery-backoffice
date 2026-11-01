using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Chat.Contracts.Authorization;
using Recovery.BackOffice.Modules.Chat.Contracts.v1.Commands;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Chat.Features.v1.Messages.SendMessage;

public static class SendMessageEndpoint
{
    internal static RouteHandlerBuilder MapSendMessageEndpoint(this IEndpointRouteBuilder endpoints)
        => endpoints.MapPost("/channels/{id:guid}/messages",
                async (Guid id, [FromBody] SendMessageBody body, IMediator mediator, CancellationToken cancellationToken) =>
                    Results.Ok(await mediator.Send(
                        new SendMessageCommand(id, body.Body, body.ParentMessageId, body.Attachments ?? []),
                        cancellationToken)))
            .WithName("SendMessage")
            .WithSummary("Send a message to a channel — supports replies (parentMessageId) and attachments")
            .RequirePermission(ChatPermissions.Messages.Send)
            .WithIdempotency();

    public sealed record SendMessageBody(
        string? Body,
        Guid? ParentMessageId,
        IReadOnlyList<SendMessageAttachmentInput>? Attachments);
}

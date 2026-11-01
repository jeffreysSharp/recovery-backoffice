using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Chat.Contracts.v1.DTOs;
using Recovery.BackOffice.Modules.Chat.Contracts.v1.Queries;
using Recovery.BackOffice.Modules.Chat.Data;
using Recovery.BackOffice.Modules.Chat.Features.v1.Internal;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Chat.Features.v1.Channels.GetChannelById;

public sealed class GetChannelByIdQueryHandler(
    ChatDbContext db,
    ICurrentUser currentUser)
    : IQueryHandler<GetChannelByIdQuery, ChannelDto>
{
    public async ValueTask<ChannelDto> Handle(GetChannelByIdQuery q, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(q);
        var userId = currentUser.GetUserId();
        if (userId == Guid.Empty) throw new UnauthorizedException("no current user");
        var currentUserId = userId.ToString();

        var channel = await db.Channels.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == q.ChannelId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("Channel not found.");

        // Private channels & DMs: must be a member. Public channels: anyone with View can see them.
        if (channel.IsPrivate)
        {
            channel.RequireMember(currentUserId);
        }

        var member = channel.Members.FirstOrDefault(m => string.Equals(m.UserId, currentUserId, StringComparison.Ordinal));
        int unread = 0;
        if (member is not null)
        {
            unread = await db.Messages.AsNoTracking()
                .Where(m => m.ChannelId == channel.Id
                    && m.DeletedAtUtc == null
                    && (member.LastReadMessageId == null || m.Id.ToString().CompareTo(member.LastReadMessageId.Value.ToString()) > 0))
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        return channel.ToDto(unread);
    }
}

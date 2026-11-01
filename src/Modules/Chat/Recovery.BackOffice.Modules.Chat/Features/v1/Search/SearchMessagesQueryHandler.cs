using System.Collections.ObjectModel;
using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.Modules.Chat.Contracts.v1.DTOs;
using Recovery.BackOffice.Modules.Chat.Contracts.v1.Queries;
using Recovery.BackOffice.Modules.Chat.Data;
using Recovery.BackOffice.Modules.Chat.Domain;
using Recovery.BackOffice.Modules.Chat.Features.v1.Internal;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Chat.Features.v1.Search;

public sealed class SearchMessagesQueryHandler(
    ChatDbContext db,
    ICurrentUser currentUser,
    IMediator mediator)
    : IQueryHandler<SearchMessagesQuery, ReadOnlyCollection<MessageDto>>
{
    public async ValueTask<ReadOnlyCollection<MessageDto>> Handle(
        SearchMessagesQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        var userId = currentUser.GetUserId();
        if (userId == Guid.Empty) throw new UnauthorizedException("no current user");
        var currentUserId = userId.ToString();

        int page = Math.Max(1, query.Page);
        int pageSize = Math.Clamp(query.PageSize, 1, 100);

        // Leakage guard: scope the search to channels the caller is a member of (intersected with the
        // requested channel if any) so cross-channel results never reach non-members.
        IQueryable<Guid> memberChannelIds = db.Channels.AsNoTracking()
            .Where(c => c.Members.Any(m => m.UserId == currentUserId))
            .Select(c => c.Id);

        if (query.ChannelId is { } scopedChannelId)
        {
            memberChannelIds = memberChannelIds.Where(id => id == scopedChannelId);
        }

        var allowedChannelIds = await memberChannelIds.ToListAsync(cancellationToken).ConfigureAwait(false);
        if (allowedChannelIds.Count == 0)
        {
            return new List<MessageDto>().AsReadOnly();
        }

        // Parameterized substring search works without a paid/full-text service.
        // Membership scope and tenant/soft-delete filters remain enforced by EF.
        int offset = PaginationExtensions.GetOffset(page, pageSize);
        var rows = await db.Messages
            .Where(m => allowedChannelIds.Contains(m.ChannelId)
                && m.DeletedAtUtc == null && EF.Functions.Like(EF.Functions.Collate(m.Body, "Latin1_General_100_CI_AS"), $"%{query.Q}%"))
            .OrderByDescending(m => m.CreatedAtUtc).ThenByDescending(m => m.Id)
            .Skip(offset).Take(pageSize)
            .AsNoTracking()
            .Include(m => m.Attachments)
            .Include(m => m.Mentions)
            .Include(m => m.Reactions)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var dtos = rows.Select(m => m.ToDto()).ToList();
        var resolved = await ChatAttachmentUrls.ResolveAsync(dtos, mediator, cancellationToken).ConfigureAwait(false);
        return resolved.AsReadOnly();
    }
}

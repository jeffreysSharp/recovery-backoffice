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

namespace Recovery.BackOffice.Modules.Chat.Features.v1.Channels.DiscoverChannels;

public sealed class DiscoverChannelsQueryHandler(
    ChatDbContext db,
    ICurrentUser currentUser)
    : IQueryHandler<DiscoverChannelsQuery, ReadOnlyCollection<ChannelDto>>
{
    public async ValueTask<ReadOnlyCollection<ChannelDto>> Handle(DiscoverChannelsQuery q, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(q);
        var userId = currentUser.GetUserId();
        if (userId == Guid.Empty) throw new UnauthorizedException("no current user");
        var currentUserId = userId.ToString();

        int page = Math.Max(1, q.Page);
        int pageSize = Math.Clamp(q.PageSize, 1, 200);

        var query = db.Channels.AsNoTracking()
            .Where(c => c.Type == ChannelType.Channel
                     && !c.IsPrivate
                     && !c.Members.Any(m => m.UserId == currentUserId));

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var term = q.Search.Trim();
            query = query.Where(c =>
                EF.Functions.Like(EF.Functions.Collate(c.Name!, "Latin1_General_100_CI_AS"), $"%{term}%")
                || EF.Functions.Like(EF.Functions.Collate(c.Slug!, "Latin1_General_100_CI_AS"), $"%{term}%"));
        }

        var channels = await query
            .OrderByDescending(c => c.LastMessageAtUtc ?? c.CreatedAtUtc)
            .Skip(PaginationExtensions.GetOffset(page, pageSize))
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return channels.Select(c => c.ToDto()).ToList().AsReadOnly();
    }
}

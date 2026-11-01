using System.Collections.ObjectModel;
using Recovery.BackOffice.Modules.Chat.Contracts.v1.DTOs;
using Mediator;

namespace Recovery.BackOffice.Modules.Chat.Contracts.v1.Queries;

public sealed record ListMyChannelsQuery(int Page = 1, int PageSize = 50)
    : IQuery<ReadOnlyCollection<ChannelDto>>;

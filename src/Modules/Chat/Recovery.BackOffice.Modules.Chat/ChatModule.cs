using Asp.Versioning;
using FluentValidation;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Constants;
using Recovery.BackOffice.BuildingBlocks.Web.Modules;
using Recovery.BackOffice.BuildingBlocks.Web.Realtime;
using Recovery.BackOffice.Modules.Chat.Contracts.Authorization;
using Recovery.BackOffice.Modules.Chat.Data;
using Recovery.BackOffice.Modules.Chat.Features.v1.Channels.AddChannelMembers;
using Recovery.BackOffice.Modules.Chat.Features.v1.Channels.ArchiveChannel;
using Recovery.BackOffice.Modules.Chat.Features.v1.Channels.CreateChannel;
using Recovery.BackOffice.Modules.Chat.Features.v1.Channels.DiscoverChannels;
using Recovery.BackOffice.Modules.Chat.Features.v1.Channels.FindOrCreateDm;
using Recovery.BackOffice.Modules.Chat.Features.v1.Channels.GetChannelById;
using Recovery.BackOffice.Modules.Chat.Features.v1.Channels.ListMyChannels;
using Recovery.BackOffice.Modules.Chat.Features.v1.Channels.MarkChannelRead;
using Recovery.BackOffice.Modules.Chat.Features.v1.Channels.RemoveChannelMember;
using Recovery.BackOffice.Modules.Chat.Features.v1.Channels.RestoreChannel;
using Recovery.BackOffice.Modules.Chat.Features.v1.Channels.UpdateChannel;
using Recovery.BackOffice.Modules.Chat.Features.v1.Messages.DeleteMessage;
using Recovery.BackOffice.Modules.Chat.Features.v1.Messages.EditMessage;
using Recovery.BackOffice.Modules.Chat.Features.v1.Messages.GetPinnedMessages;
using Recovery.BackOffice.Modules.Chat.Features.v1.Messages.ListChannelMessages;
using Recovery.BackOffice.Modules.Chat.Features.v1.Messages.ListMessageReplies;
using Recovery.BackOffice.Modules.Chat.Features.v1.Messages.PinMessage;
using Recovery.BackOffice.Modules.Chat.Features.v1.Messages.SendMessage;
using Recovery.BackOffice.Modules.Chat.Features.v1.Messages.UnpinMessage;
using Recovery.BackOffice.Modules.Chat.Features.v1.Reactions.AddReaction;
using Recovery.BackOffice.Modules.Chat.Features.v1.Reactions.RemoveReaction;
using Recovery.BackOffice.Modules.Chat.Features.v1.Search;
using Recovery.BackOffice.Modules.Chat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Recovery.BackOffice.Modules.Chat;

/// <summary>
/// Chat module: Slack-style messaging (DMs + group DMs + named channels). Module Order 800 places
/// it after Notifications (750) so the Notifications module can register integration-event handlers
/// before Chat starts publishing.
/// </summary>
public sealed class ChatModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(ChatPermissions.All);

        builder.Services.AddRecoveryDbContext<ChatDbContext>();
        builder.Services.AddScoped<IDbInitializer, ChatDbInitializer>();
        builder.Services.AddValidatorsFromAssembly(typeof(ChatModule).Assembly);

        // Realtime adapters consumed by AppHub (BuildingBlocks/Web). These let the shared hub
        // verify channel membership and pre-join channel groups without depending on Chat.
        builder.Services.AddScoped<IChannelMembershipChecker, ChannelMembershipChecker>();
        builder.Services.AddScoped<IUserChannelLookup, UserChannelLookup>();

        // @username resolution for SendMessage. Goes through Identity contracts so the user
        // directory stays the single source of truth.
        builder.Services.AddScoped<IMentionResolver, MentionResolver>();

        // File attachments: members attach+read, only the uploader deletes. Registered as
        // IFileAccessPolicy so Files endpoints route through it for OwnerType=ChatChannel.
        builder.Services.AddScoped<Recovery.BackOffice.Modules.Files.Contracts.IFileAccessPolicy, Authorization.ChatChannelFileAccessPolicy>();

        builder.Services.AddHealthChecks().AddDbContextCheck<ChatDbContext>(
            name: "db:chat",
            failureStatus: HealthStatus.Unhealthy);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = endpoints.MapGroup("api/v{version:apiVersion}/chat")
            .WithTags("Chat")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        // Channel reads — literal routes first
        group.MapListMyChannelsEndpoint();           // GET /channels
        group.MapDiscoverChannelsEndpoint();         // GET /channels/discover

        // Channel lifecycle
        group.MapCreateChannelEndpoint();
        group.MapFindOrCreateDmEndpoint();           // POST /dms — literal route comes before /{id}
        group.MapRestoreChannelEndpoint();           // literal /restore must precede catch-alls
        group.MapAddChannelMembersEndpoint();
        group.MapRemoveChannelMemberEndpoint();
        group.MapMarkChannelReadEndpoint();
        group.MapUpdateChannelEndpoint();
        group.MapArchiveChannelEndpoint();
        group.MapGetChannelByIdEndpoint();           // GET /channels/{id} — must follow literal routes

        // Messages
        group.MapListChannelMessagesEndpoint();      // GET /channels/{id}/messages
        group.MapListMessageRepliesEndpoint();       // GET /messages/{id}/replies
        group.MapGetPinnedMessagesEndpoint();        // GET /channels/{id}/pinned
        group.MapSendMessageEndpoint();              // POST /channels/{id}/messages
        group.MapEditMessageEndpoint();              // PUT /messages/{id}
        group.MapDeleteMessageEndpoint();            // DELETE /messages/{id}
        group.MapPinMessageEndpoint();               // POST /messages/{id}/pin
        group.MapUnpinMessageEndpoint();             // DELETE /messages/{id}/pin

        // Reactions
        group.MapAddReactionEndpoint();              // POST /messages/{id}/reactions
        group.MapRemoveReactionEndpoint();           // DELETE /messages/{id}/reactions/{emoji}

        // Search
        group.MapSearchMessagesEndpoint();           // GET /search
    }
}

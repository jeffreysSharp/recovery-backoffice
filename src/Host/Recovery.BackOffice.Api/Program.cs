using Recovery.BackOffice.BuildingBlocks.Eventing;
using Recovery.BackOffice.BuildingBlocks.Web;
using Recovery.BackOffice.BuildingBlocks.Web.Modules;
using Recovery.BackOffice.Modules.Auditing;
using Recovery.BackOffice.Modules.Identity;
using Recovery.BackOffice.Modules.Identity.Contracts.v1.Tokens.TokenGeneration;
using Recovery.BackOffice.Modules.Identity.Features.v1.Tokens.TokenGeneration;
using Recovery.BackOffice.Modules.Multitenancy;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.GetTenantStatus;
using Recovery.BackOffice.Modules.Webhooks;
using Recovery.BackOffice.Modules.Billing;
using Recovery.BackOffice.Modules.Catalog;
using Recovery.BackOffice.Modules.Tickets;
using Recovery.BackOffice.Modules.Multitenancy.Features.v1.GetTenantStatus;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Serialize enums as string names (reads still accept names or integers). [Flags] enums (AuditTag, BodyCapture)
// opt back to numeric via their own NumericEnumConverter since comma-joined flag strings break bitwise consumers. Frontends mirror this as string unions.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

if (builder.Environment.IsProduction())
{
    static void Require(IConfiguration config, string key)
    {
        if (string.IsNullOrWhiteSpace(config[key]))
        {
            throw new InvalidOperationException($"Missing required configuration '{key}' in Production.");
        }
    }

    var config = builder.Configuration;
    Require(config, "DatabaseOptions:ConnectionString");
    Require(config, "CachingOptions:Redis");
    Require(config, "JwtOptions:SigningKey");

    // Password-reset / confirmation links are built from this; without it those flows can only 500.
    // Absolute http(s) only: on Unix, Uri.TryCreate also accepts "/path" as a file:// URI.
    if (!Uri.TryCreate(config["FrontendOptions:DefaultOrigin"], UriKind.Absolute, out Uri? defaultOrigin)
        || (defaultOrigin.Scheme != Uri.UriSchemeHttp && defaultOrigin.Scheme != Uri.UriSchemeHttps))
    {
        throw new InvalidOperationException(
            "Missing required configuration 'FrontendOptions:DefaultOrigin' in Production: set it to your dashboard URL, e.g. \"https://app.example.com\".");
    }
}

builder.Services.AddMediator(o =>
{
    o.ServiceLifetime = ServiceLifetime.Scoped;
    o.Assemblies = [
        typeof(GenerateTokenCommand),
        typeof(GenerateTokenCommandHandler),
        typeof(GetTenantStatusQuery),
        typeof(GetTenantStatusQueryHandler),
        typeof(Recovery.BackOffice.Modules.Auditing.Contracts.AuditEnvelope),
        typeof(Recovery.BackOffice.Modules.Auditing.Persistence.AuditDbContext),
        typeof(Recovery.BackOffice.Modules.Webhooks.Contracts.v1.CreateWebhookSubscription.CreateWebhookSubscriptionCommand),
        typeof(Recovery.BackOffice.Modules.Webhooks.WebhooksModule),
        typeof(Recovery.BackOffice.Modules.Billing.Contracts.BillingContractsMarker),
        typeof(Recovery.BackOffice.Modules.Billing.BillingModule),
        typeof(Recovery.BackOffice.Modules.Catalog.Contracts.CatalogContractsMarker),
        typeof(Recovery.BackOffice.Modules.Catalog.CatalogModule),
        typeof(Recovery.BackOffice.Modules.Tickets.Contracts.TicketsContractsMarker),
        typeof(Recovery.BackOffice.Modules.Tickets.TicketsModule),
        typeof(Recovery.BackOffice.Modules.Files.Contracts.v1.Commands.RequestUploadUrlCommand),
        typeof(Recovery.BackOffice.Modules.Files.FilesModule),
        typeof(Recovery.BackOffice.Modules.Chat.Contracts.v1.Commands.CreateChannelCommand),
        typeof(Recovery.BackOffice.Modules.Chat.ChatModule),
        typeof(Recovery.BackOffice.Modules.Notifications.Contracts.v1.Commands.MarkNotificationReadCommand),
        typeof(Recovery.BackOffice.Modules.Notifications.NotificationsModule)];
});

var moduleAssemblies = new Assembly[]
{
    typeof(IdentityModule).Assembly,
    typeof(MultitenancyModule).Assembly,
    typeof(AuditingModule).Assembly,
    typeof(Recovery.BackOffice.Modules.Files.FilesModule).Assembly,
    typeof(WebhooksModule).Assembly,
    typeof(BillingModule).Assembly,
    typeof(CatalogModule).Assembly,
    typeof(TicketsModule).Assembly,
    typeof(Recovery.BackOffice.Modules.Chat.ChatModule).Assembly,
    typeof(Recovery.BackOffice.Modules.Notifications.NotificationsModule).Assembly,
};

builder.AddRecoveryPlatform(o =>
{
    o.EnableCaching = true;
    o.EnableMailing = true;
    o.EnableJobs = true;
    o.EnableQuotas = true;
    o.EnableSse = true;
    o.EnableRealtime = true;
});

// The transactional outbox is framework infrastructure with exactly one owner
// (EventingDbContext), so the host registers it once for every module (issue #1349).
builder.Services.AddEventingCore(builder.Configuration);

builder.AddModules(moduleAssemblies);

// Self-heal deployments carrying retired per-module `{module}-outbox-dispatcher` Hangfire recurring jobs
// (the outbox is now dispatched by OutboxDispatcherHostedService). No-op once the storage is clean.
builder.Services.AddHostedService<Recovery.BackOffice.Api.OrphanedOutboxRecurringJobCleanupService>();

// Demo data is provisioned by the DbMigrator's `seed-demo` verb, not the API — the API never mutates data on startup.
// See src/Host/Recovery.BackOffice.DbMigrator/README.md.

var app = builder.Build();

app.UseRecoveryMultiTenantDatabases();
app.UseRecoveryPlatform(p =>
{
    p.MapModules = true;
    p.ServeStaticFiles = true;
    p.UseQuotas = true;
    p.MapSseEndpoints = true;
    p.MapRealtime = true;
});

app.MapGet("/", () => Results.Ok(new { message = "hello world!" }))
   .WithTags("PlayGround")
   .AllowAnonymous();
await app.RunAsync();
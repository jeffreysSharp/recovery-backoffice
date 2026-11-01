using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Recovery.BackOffice.Migrations.SqlServer;

internal static class DesignTimeContext
{
    public static T Create<T>() where T : DbContext
    {
        var connection = Environment.GetEnvironmentVariable("DatabaseOptions__ConnectionString")
            ?? @"Server=.\SQLEXPRESS;Database=RecoveryBackOfficeLocal;Integrated Security=true;TrustServerCertificate=true";
        var options = new DbContextOptionsBuilder<T>().UseSqlServer(connection,
            sql => sql.MigrationsAssembly(typeof(DesignTimeContext).Assembly.GetName().Name)).Options;
        if (typeof(T).Name is "TenantDbContext" or "BillingDbContext")
            return (T)Activator.CreateInstance(typeof(T), options)!;
        return (T)Activator.CreateInstance(typeof(T), new TenantAccessor(), options,
            Options.Create(new DatabaseOptions { Provider = DbProviders.MSSQL, ConnectionString = connection,
                MigrationsAssembly = typeof(DesignTimeContext).Assembly.GetName().Name! }), new DesignEnvironment())!;
    }

    private sealed class TenantAccessor : IMultiTenantContextAccessor<AppTenantInfo>
    {
        public IMultiTenantContext<AppTenantInfo> MultiTenantContext { get; } =
            new MultiTenantContext<AppTenantInfo>(new AppTenantInfo());
        IMultiTenantContext IMultiTenantContextAccessor.MultiTenantContext => MultiTenantContext;
    }

    private sealed class DesignEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "Recovery.BackOffice.Migrations.SqlServer";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}

public sealed class IdentityDesignFactory : IDesignTimeDbContextFactory<Recovery.BackOffice.Modules.Identity.Data.IdentityDbContext>
{
    public Recovery.BackOffice.Modules.Identity.Data.IdentityDbContext CreateDbContext(string[] args) => DesignTimeContext.Create<Recovery.BackOffice.Modules.Identity.Data.IdentityDbContext>();
}

public sealed class TenantDesignFactory : IDesignTimeDbContextFactory<Recovery.BackOffice.Modules.Multitenancy.Data.TenantDbContext>
{
    public Recovery.BackOffice.Modules.Multitenancy.Data.TenantDbContext CreateDbContext(string[] args) => DesignTimeContext.Create<Recovery.BackOffice.Modules.Multitenancy.Data.TenantDbContext>();
}

public sealed class AuditDesignFactory : IDesignTimeDbContextFactory<Recovery.BackOffice.Modules.Auditing.Persistence.AuditDbContext>
{
    public Recovery.BackOffice.Modules.Auditing.Persistence.AuditDbContext CreateDbContext(string[] args) => DesignTimeContext.Create<Recovery.BackOffice.Modules.Auditing.Persistence.AuditDbContext>();
}

public sealed class EventingDesignFactory : IDesignTimeDbContextFactory<Recovery.BackOffice.BuildingBlocks.Eventing.Persistence.EventingDbContext>
{
    public Recovery.BackOffice.BuildingBlocks.Eventing.Persistence.EventingDbContext CreateDbContext(string[] args) => DesignTimeContext.Create<Recovery.BackOffice.BuildingBlocks.Eventing.Persistence.EventingDbContext>();
}

public sealed class BillingDesignFactory : IDesignTimeDbContextFactory<Recovery.BackOffice.Modules.Billing.Data.BillingDbContext>
{
    public Recovery.BackOffice.Modules.Billing.Data.BillingDbContext CreateDbContext(string[] args) => DesignTimeContext.Create<Recovery.BackOffice.Modules.Billing.Data.BillingDbContext>();
}

public sealed class CatalogDesignFactory : IDesignTimeDbContextFactory<Recovery.BackOffice.Modules.Catalog.Data.CatalogDbContext>
{
    public Recovery.BackOffice.Modules.Catalog.Data.CatalogDbContext CreateDbContext(string[] args) => DesignTimeContext.Create<Recovery.BackOffice.Modules.Catalog.Data.CatalogDbContext>();
}

public sealed class TicketsDesignFactory : IDesignTimeDbContextFactory<Recovery.BackOffice.Modules.Tickets.Data.TicketsDbContext>
{
    public Recovery.BackOffice.Modules.Tickets.Data.TicketsDbContext CreateDbContext(string[] args) => DesignTimeContext.Create<Recovery.BackOffice.Modules.Tickets.Data.TicketsDbContext>();
}

public sealed class ChatDesignFactory : IDesignTimeDbContextFactory<Recovery.BackOffice.Modules.Chat.Data.ChatDbContext>
{
    public Recovery.BackOffice.Modules.Chat.Data.ChatDbContext CreateDbContext(string[] args) => DesignTimeContext.Create<Recovery.BackOffice.Modules.Chat.Data.ChatDbContext>();
}

public sealed class NotificationsDesignFactory : IDesignTimeDbContextFactory<Recovery.BackOffice.Modules.Notifications.Data.NotificationsDbContext>
{
    public Recovery.BackOffice.Modules.Notifications.Data.NotificationsDbContext CreateDbContext(string[] args) => DesignTimeContext.Create<Recovery.BackOffice.Modules.Notifications.Data.NotificationsDbContext>();
}

public sealed class FilesDesignFactory : IDesignTimeDbContextFactory<Recovery.BackOffice.Modules.Files.Data.FilesDbContext>
{
    public Recovery.BackOffice.Modules.Files.Data.FilesDbContext CreateDbContext(string[] args) => DesignTimeContext.Create<Recovery.BackOffice.Modules.Files.Data.FilesDbContext>();
}

public sealed class WebhookDesignFactory : IDesignTimeDbContextFactory<Recovery.BackOffice.Modules.Webhooks.Data.WebhookDbContext>
{
    public Recovery.BackOffice.Modules.Webhooks.Data.WebhookDbContext CreateDbContext(string[] args) => DesignTimeContext.Create<Recovery.BackOffice.Modules.Webhooks.Data.WebhookDbContext>();
}

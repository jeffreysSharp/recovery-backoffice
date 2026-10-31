using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Recovery.BackOffice.Multitenancy;

public static class Extensions
{
    public static WebApplication UseRecoveryMultiTenantDatabases(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);
        app.UseMultiTenant();

        return app;
    }
}

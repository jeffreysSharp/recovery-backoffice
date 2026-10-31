using Microsoft.AspNetCore.Builder;

namespace Recovery.BackOffice.BuildingBlocks.Web.Security;

public static class SecurityExtensions
{
    public static IApplicationBuilder UseRecoverySecurityHeaders(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
using Recovery.BackOffice.Modules.Auditing;
using Recovery.BackOffice.Modules.Identity;
using Recovery.BackOffice.Modules.Multitenancy;
using System.Reflection;

namespace Recovery.BackOffice.Generic.Tests.Architecture;

/// <summary>
/// Discovers all Recovery module assemblies for use in generic architecture tests.
/// </summary>
internal static class ModuleAssemblyDiscovery
{
    private static readonly Assembly[] _cached = Discover();

    public static Assembly[] GetModuleAssemblies() => _cached;

    private static Assembly[] Discover()
    {
        // Force-load seed assemblies
        _ = typeof(AuditingModule);
        _ = typeof(IdentityModule);
        _ = typeof(MultitenancyModule);

        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a =>
            {
                var name = a.GetName().Name ?? string.Empty;
                return name.StartsWith("Recovery.BackOffice.Modules.", StringComparison.Ordinal)
                       && !name.EndsWith(".Contracts", StringComparison.Ordinal);
            })
            .OrderBy(a => a.GetName().Name, StringComparer.Ordinal)
            .ToArray();
    }
}

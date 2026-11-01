using Recovery.BackOffice.BuildingBlocks.Shared.Constants;

namespace Recovery.BackOffice.Modules.Multitenancy.Contracts.Authorization;

public static class MultitenancyPermissions
{
    public static class Tenants
    {
        public const string Resource = nameof(Tenants);
        public const string View                = $"Permissions.{Resource}.View";
        public const string Create              = $"Permissions.{Resource}.Create";
        public const string Update              = $"Permissions.{Resource}.Update";
        public const string UpgradeSubscription = $"Permissions.{Resource}.UpgradeSubscription";
        public const string ViewTheme           = $"Permissions.{Resource}.ViewTheme";
        public const string UpdateTheme         = $"Permissions.{Resource}.UpdateTheme";
    }

    public static IReadOnlyList<RecoveryPermission> All { get; } =
    [
        new("View Agencies",               ActionConstants.View,                Tenants.Resource, IsRoot: true),
        new("Create Agencies",             ActionConstants.Create,              Tenants.Resource, IsRoot: true),
        new("Update Agencies",             ActionConstants.Update,              Tenants.Resource, IsRoot: true),
        new("Upgrade Agency Subscription",ActionConstants.UpgradeSubscription, Tenants.Resource, IsRoot: true),
        new("View Agency Theme",          "ViewTheme",                         Tenants.Resource, IsBasic: true),
        new("Update Agency Theme",        "UpdateTheme",                       Tenants.Resource),
    ];
}

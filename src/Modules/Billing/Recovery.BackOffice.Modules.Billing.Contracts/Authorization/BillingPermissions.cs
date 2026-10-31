using Recovery.BackOffice.BuildingBlocks.Shared.Constants;

namespace Recovery.BackOffice.Modules.Billing.Contracts.Authorization;

public static class BillingPermissions
{
    public const string Resource = "Billing";
    public const string View   = $"Permissions.{Resource}.View";
    public const string Manage = $"Permissions.{Resource}.Manage";

    public static IReadOnlyList<RecoveryPermission> All { get; } =
    [
        new("View Billing",   ActionConstants.View, Resource, IsBasic: true),
        new("Manage Billing", "Manage",             Resource),
    ];
}

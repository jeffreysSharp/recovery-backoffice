namespace Recovery.BackOffice.BuildingBlocks.Web.Modules;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class RecoveryModuleAttribute : Attribute
{
    public RecoveryModuleAttribute(Type moduleType, int order = 0)
    {
        ArgumentNullException.ThrowIfNull(moduleType);

        if (!typeof(IModule).IsAssignableFrom(moduleType)
            || !moduleType.IsClass
            || moduleType.IsAbstract
            || moduleType.ContainsGenericParameters
            || moduleType.GetConstructor(Type.EmptyTypes) is null)
        {
            throw new ArgumentException(
                "Module must be a concrete IModule with a public parameterless constructor.",
                nameof(moduleType));
        }

        ModuleType = moduleType;
        Order = order;
    }

    public Type ModuleType { get; }

    public int Order { get; }
}

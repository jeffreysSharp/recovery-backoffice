using System;

namespace Recovery.BackOffice.BuildingBlocks.Web.Modules;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class RecoveryModuleAttribute : Attribute
{
    public Type ModuleType { get; }

    /// <summary>
    /// Optional ordering hint that allows hosts to control module startup sequencing.
    /// Lower numbers execute first.
    /// </summary>
    public int Order { get; }

    public RecoveryModuleAttribute(Type moduleType, int order = 0)
    {
        ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
        Order = order;
    }
}
using Microsoft.AspNetCore.Identity;

namespace Recovery.BackOffice.Modules.Identity.Domain;

public class RecoveryRole : IdentityRole
{
    public string? Description { get; set; }

    public RecoveryRole(string name, string? description = null)
        : base(name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Description = description;
        NormalizedName = name.ToUpperInvariant();
    }
}
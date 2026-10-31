using Microsoft.AspNetCore.Identity;

namespace Recovery.BackOffice.Identity.Domain;

public class RecoveryRoleClaim : IdentityRoleClaim<string>
{
    public string? CreatedBy { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
}
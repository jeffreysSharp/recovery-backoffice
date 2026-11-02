using System.Net;
using Finbuckle.MultiTenant.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.BuildingBlocks.Shared.Constants;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Modules.Identity.Contracts.DTOs;
using Recovery.BackOffice.Modules.Identity.Contracts.Services;
using Recovery.BackOffice.Modules.Identity.Data;
using Recovery.BackOffice.Modules.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Identity.Services;

internal sealed class UserRoleService(
    UserManager<RecoveryUser> userManager,
    RoleManager<RecoveryRole> roleManager,
    IdentityDbContext db,
    IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IUserRoleService
{
    public async Task<string> AssignRolesAsync(string userId, List<UserRoleDto> userRoles, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("user not found");

        await ValidateAdminRoleChangeAsync(user, userRoles);

        var assignedRoles = await ProcessRoleAssignmentsAsync(user, userRoles);

        await RaiseRolesAssignedEventAsync(user, assignedRoles, cancellationToken);

        // Any role mutation (add or remove) invalidates the cached permission set; flush
        // unconditionally rather than gating on assignedRoles, which only tracks additions.
        await userPermissionService.InvalidatePermissionCacheAsync(userId, cancellationToken).ConfigureAwait(false);

        return "User Roles Updated Successfully.";
    }

    public async Task<List<UserRoleDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("user not found");

        var roles = await roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken)
            ?? throw new NotFoundException("roles not found");

        // Single membership query instead of one IsInRoleAsync round-trip per role.
        var memberships = await userManager.GetRolesAsync(user);
        var membershipSet = new HashSet<string>(memberships, StringComparer.OrdinalIgnoreCase);

        var userRoles = new List<UserRoleDto>();
        foreach (var role in roles)
        {
            userRoles.Add(new UserRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Description = role.Description,
                Enabled = membershipSet.Contains(role.Name!)
            });
        }

        return userRoles;
    }

    private async Task ValidateAdminRoleChangeAsync(RecoveryUser user, List<UserRoleDto> userRoles)
    {
        bool isRemovingAdminRole = userRoles.Exists(a => !a.Enabled && a.RoleName == RoleConstants.Admin);
        if (!isRemovingAdminRole)
        {
            return;
        }

        bool userIsAdmin = await userManager.IsInRoleAsync(user, RoleConstants.Admin);
        if (!userIsAdmin)
        {
            return;
        }

        // Administrators cannot demote themselves — they would lose access immediately on the next request,
        // and would need another admin to restore them.
        var actorId = currentUser.GetUserId();
        if (actorId != Guid.Empty && string.Equals(actorId.ToString(), user.Id, StringComparison.Ordinal))
        {
            throw new CustomException(
                "Administrators cannot remove their own admin role.",
                Array.Empty<string>(),
                HttpStatusCode.BadRequest);
        }

        // The root tenant's seed admin is the framework's last-resort recovery account.
        if (IsRootTenantAdmin(user))
        {
            throw new ForbiddenException("The root agency administrator cannot be demoted.");
        }

        // After this removal, at least one admin must remain in the tenant — matches
        // the "at least one active administrator" invariant enforced on user deactivation.
        await EnsureMinimumAdminCountAsync();
    }

    private bool IsRootTenantAdmin(RecoveryUser user)
    {
        return user.Email == MultitenancyConstants.Root.EmailAddress
            && multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Id == MultitenancyConstants.Root.Id;
    }

    private async Task EnsureMinimumAdminCountAsync()
    {
        int adminCount = (await userManager.GetUsersInRoleAsync(RoleConstants.Admin)).Count;
        if (adminCount <= 1)
        {
            throw new CustomException(
                "Agency must retain at least one administrator.",
                Array.Empty<string>(),
                HttpStatusCode.BadRequest);
        }
    }

    private async Task<List<string>> ProcessRoleAssignmentsAsync(RecoveryUser user, List<UserRoleDto> userRoles)
    {
        var assignedRoles = new List<string>();

        foreach (var userRole in userRoles)
        {
            if (await roleManager.FindByNameAsync(userRole.RoleName!) is null)
            {
                continue;
            }

            if (userRole.Enabled)
            {
                if (!await userManager.IsInRoleAsync(user, userRole.RoleName!))
                {
                    await userManager.AddToRoleAsync(user, userRole.RoleName!);
                    assignedRoles.Add(userRole.RoleName!);
                }
            }
            else
            {
                await userManager.RemoveFromRoleAsync(user, userRole.RoleName!);
            }
        }

        return assignedRoles;
    }

    private async Task RaiseRolesAssignedEventAsync(RecoveryUser user, List<string> assignedRoles, CancellationToken cancellationToken)
    {
        if (assignedRoles.Count == 0)
        {
            return;
        }

        var tenantId = multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Id;
        user.RecordRolesAssigned(assignedRoles, tenantId);
        await db.SaveChangesAsync(cancellationToken);
    }
}
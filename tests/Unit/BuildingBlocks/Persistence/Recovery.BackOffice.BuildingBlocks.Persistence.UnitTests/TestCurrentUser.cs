using System.Security.Claims;
using Recovery.BackOffice.BuildingBlocks.Core.Context;

namespace Recovery.BackOffice.BuildingBlocks.Persistence.UnitTests;

internal sealed class TestCurrentUser(Guid userId = default, bool authenticated = false) : ICurrentUser
{
    public string? Name => null;

    public Guid GetUserId() => userId;

    public string? GetUserEmail() => null;

    public string? GetTenant() => null;

    public bool IsAuthenticated() => authenticated;

    public bool IsInRole(string role) => false;

    public IEnumerable<Claim> GetUserClaims() => [];
}

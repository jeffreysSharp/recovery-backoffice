using System.Security.Claims;
using Recovery.BackOffice.BuildingBlocks.Shared.Constants;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Claims;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Shared.UnitTests;

public sealed class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void ClaimAccessors_Should_ReadMappedValues_When_ClaimsExist()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "user-id"),
             new Claim(ClaimTypes.Email, "user@example.test"),
             new Claim(ClaimTypes.Name, "First"),
             new Claim(ClaimTypes.Surname, "Last"),
             new Claim(ClaimTypes.MobilePhone, "+5511999999999"),
             new Claim(CustomClaims.Fullname, "First Last"),
             new Claim(CustomClaims.Tenant, "tenant-a")]));

        Assert.Equal("user-id", principal.GetUserId());
        Assert.Equal("user@example.test", principal.GetEmail());
        Assert.Equal("First", principal.GetFirstName());
        Assert.Equal("Last", principal.GetSurname());
        Assert.Equal("First Last", principal.GetFullName());
        Assert.Equal("+5511999999999", principal.GetPhoneNumber());
        Assert.Equal("tenant-a", principal.GetTenant());
    }

    [Fact]
    public void ClaimAccessors_Should_ReturnNull_When_ClaimsAreMissing()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        Assert.Null(principal.GetUserId());
        Assert.Null(principal.GetEmail());
        Assert.Null(principal.GetFirstName());
        Assert.Null(principal.GetSurname());
        Assert.Null(principal.GetFullName());
        Assert.Null(principal.GetPhoneNumber());
        Assert.Null(principal.GetTenant());
        Assert.Null(principal.GetImageUrl());
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("/avatar.png")]
    public void GetImageUrl_Should_ReturnNull_When_ImageClaimIsNotAbsoluteUri(string image)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(CustomClaims.ImageUrl, image)]));

        Assert.Null(principal.GetImageUrl());
    }

    [Fact]
    public void GetImageUrl_Should_ReturnUri_When_ImageClaimIsAbsolute()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(CustomClaims.ImageUrl, "https://api.example.test/avatar.png")]));

        Assert.Equal(new Uri("https://api.example.test/avatar.png"), principal.GetImageUrl());
    }

    [Fact]
    public void GetExpiration_Should_ConvertUnixTimestamp_When_ClaimExists()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(CustomClaims.Expiration, "1700000000")]));

        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1700000000), principal.GetExpiration());
    }

    [Fact]
    public void GetExpiration_Should_RejectMissingClaim_When_ExpirationIsAbsent()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        Assert.Throws<InvalidOperationException>(() => principal.GetExpiration());
    }

    [Fact]
    public void GetExpiration_Should_RejectInvalidValue_When_ClaimIsNotNumeric()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(CustomClaims.Expiration, "invalid")]));

        Assert.Throws<FormatException>(() => principal.GetExpiration());
    }
}

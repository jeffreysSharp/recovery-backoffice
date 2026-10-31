using Finbuckle.MultiTenant.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Storage.Services;
using Recovery.BackOffice.Modules.Identity.Contracts.Services;
using Recovery.BackOffice.Modules.Identity.Domain;
using Recovery.BackOffice.Modules.Identity.Services;
using Recovery.BackOffice.Identity.Tests.Support;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Recovery.BackOffice.Identity.Tests.Services;

/// <summary>
/// Covers the User.Locale foundation: UpdateAsync persists the locale onto the entity and
/// GetAsync projects it back onto the DTO.
/// </summary>
public sealed class UserLocaleTests
{
    private readonly UserManager<RecoveryUser> _userManager;
    private readonly SignInManager<RecoveryUser> _signInManager;
    private readonly IStorageService _storageService;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor;

    public UserLocaleTests()
    {
        _userManager = Substitute.For<UserManager<RecoveryUser>>(
            Substitute.For<IUserStore<RecoveryUser>>(), null, null, null, null, null, null, null, null);
        _signInManager = Substitute.For<SignInManager<RecoveryUser>>(
            _userManager,
            Substitute.For<IHttpContextAccessor>(),
            Substitute.For<IUserClaimsPrincipalFactory<RecoveryUser>>(),
            Options.Create(new IdentityOptions()),
            Substitute.For<ILogger<SignInManager<RecoveryUser>>>(),
            Substitute.For<IAuthenticationSchemeProvider>(),
            Substitute.For<IUserConfirmation<RecoveryUser>>());
        _signInManager.RefreshSignInAsync(Arg.Any<RecoveryUser>()).Returns(Task.CompletedTask);
        _storageService = Substitute.For<IStorageService>();
        _tenantAccessor = Substitute.For<IMultiTenantContextAccessor<AppTenantInfo>>();
    }

    private UserProfileService CreateSut() =>
        new(_userManager, _signInManager, _storageService, _tenantAccessor,
            Substitute.For<IRequestContextService>(), new IdentityErrorDescriber());

    [Fact]
    public async Task UpdateAsync_persists_the_supplied_locale_onto_the_user()
    {
        // Arrange
        var user = new RecoveryUser { Id = "u1", Email = "u@codefi.com.br", UserName = "u" };
        _userManager.FindByIdAsync("u1").Returns(user);
        _userManager.UpdateAsync(user).Returns(IdentityResult.Success);
        var sut = CreateSut();

        // Act
        await sut.UpdateAsync("u1", "First", "Last", string.Empty, null!, false, "pt-BR", null, CancellationToken.None);

        // Assert
        user.Locale.ShouldBe("pt-BR");
    }

    [Fact]
    public async Task UpdateAsync_with_null_locale_preserves_the_existing_value()
    {
        // Arrange — a text-only edit forwards a null locale; the user already chose en-US.
        var user = new RecoveryUser { Id = "u1", Email = "u@codefi.com.br", UserName = "u", Locale = "en-US" };
        _userManager.FindByIdAsync("u1").Returns(user);
        _userManager.UpdateAsync(user).Returns(IdentityResult.Success);
        var sut = CreateSut();

        // Act
        await sut.UpdateAsync("u1", "First", "Last", string.Empty, null!, false, null, null, CancellationToken.None);

        // Assert
        user.Locale.ShouldBe("en-US");
    }

    [Fact]
    public async Task GetAsync_projects_the_persisted_locale_onto_the_dto()
    {
        // Arrange
        var user = new RecoveryUser { Id = "u1", Email = "u@codefi.com.br", UserName = "u", Locale = "pt-BR" };
        _userManager.Users.Returns(new[] { user }.AsAsyncQueryable());
        var sut = CreateSut();

        // Act
        var dto = await sut.GetAsync("u1", CancellationToken.None);

        // Assert
        dto.Locale.ShouldBe("pt-BR");
    }
}

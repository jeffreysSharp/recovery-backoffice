using Recovery.BackOffice.Identity;
using Xunit;

namespace Recovery.BackOffice.Identity.UnitTests;

public sealed class IdentityModuleTests
{
    [Fact]
    public void ConfigureServices_Should_ThrowArgumentNullException_When_BuilderIsNull()
    {
        // Arrange
        var module = new IdentityModule();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(
            () => module.ConfigureServices(null!));

        // Assert
        Assert.Equal("builder", exception.ParamName);
    }

    [Fact]
    public void ConfigureMiddleware_Should_ThrowArgumentNullException_When_AppIsNull()
    {
        // Arrange
        var module = new IdentityModule();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(
            () => module.ConfigureMiddleware(null!));

        // Assert
        Assert.Equal("app", exception.ParamName);
    }

    [Fact]
    public void MapEndpoints_Should_ThrowArgumentNullException_When_EndpointsIsNull()
    {
        // Arrange
        var module = new IdentityModule();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(
            () => module.MapEndpoints(null!));

        // Assert
        Assert.Equal("endpoints", exception.ParamName);
    }
}

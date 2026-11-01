using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Recovery.BackOffice.Framework.Tests.Persistence;

public sealed class ConnectionStringValidatorTests
{
    private static ConnectionStringValidator Build(string provider)
    {
        var options = Options.Create(new DatabaseOptions { Provider = provider });
        var logger = Substitute.For<ILogger<ConnectionStringValidator>>();
        return new ConnectionStringValidator(options, logger);
    }

    #region Happy Path

    [Fact]
    public void TryValidate_Should_ReturnTrue_When_SqlServerConnectionStringValid()
    {
        // Arrange
        var sut = Build(DbProviders.MSSQL);

        // Act
        var result = sut.TryValidate("Server=localhost;Database=recovery;User Id=sa;Password=pwd");

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void TryValidate_Should_ReturnTrue_When_MssqlConnectionStringValid()
    {
        // Arrange
        var sut = Build(DbProviders.MSSQL);

        // Act
        var result = sut.TryValidate("Server=localhost;Database=recovery;User Id=sa;Password=pwd;");

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void TryValidate_Should_HonorExplicitProviderOverride_When_ProvidedArgument()
    {
        // Arrange — configured provider is SqlServer, but call passes MSSQL explicitly.
        var sut = Build(DbProviders.MSSQL);

        // Act
        var result = sut.TryValidate("Server=localhost;Database=recovery;", DbProviders.MSSQL);

        // Assert
        result.ShouldBeTrue();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void TryValidate_Should_ReturnFalse_When_ProviderUnknown()
    {
        // Arrange — unknown provider falls through default arm without parsing.
        var sut = Build("SQLITE");

        // Act
        var result = sut.TryValidate("any-string");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void TryValidate_Should_ReturnFalse_When_SqlServerConnectionStringMalformed()
    {
        // Arrange
        var sut = Build(DbProviders.MSSQL);

        // Act — unknown keyword triggers ArgumentException in the builder.
        var result = sut.TryValidate("Server=localhost;ThisKeyIsNotValid=oops");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void TryValidate_Should_ReturnTrue_When_EmptyOrWhitespace()
    {
        // Arrange — builders accept empty/whitespace as a valid (empty) connection string.
        var sut = Build(DbProviders.MSSQL);

        // Act & Assert
        sut.TryValidate(string.Empty).ShouldBeTrue();
        sut.TryValidate("   ").ShouldBeTrue();
    }

    #endregion
}

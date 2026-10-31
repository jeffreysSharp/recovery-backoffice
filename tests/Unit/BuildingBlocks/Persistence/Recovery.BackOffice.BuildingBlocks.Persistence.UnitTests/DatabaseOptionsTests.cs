using System.ComponentModel.DataAnnotations;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Persistence.UnitTests;

public sealed class DatabaseOptionsTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_Should_RejectConnectionString_When_ValueIsEmpty(string? connectionString)
    {
        // Arrange
        var options = new DatabaseOptions { ConnectionString = connectionString! };

        // Act
        var results = options.Validate(new ValidationContext(options)).ToArray();

        // Assert
        var result = Assert.Single(results);
        Assert.Contains(nameof(DatabaseOptions.ConnectionString), result.MemberNames);
    }

    [Fact]
    public void Validate_Should_AcceptConfiguration_When_ConnectionStringIsProvided()
    {
        var options = new DatabaseOptions
        {
            ConnectionString = "Server=localhost;Database=RecoveryTests;Integrated Security=True"
        };

        Assert.Empty(options.Validate(new ValidationContext(options)));
    }
}

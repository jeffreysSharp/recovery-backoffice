using System.Net;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Core.UnitTests;

public sealed class CustomExceptionTests
{
    [Fact]
    public void Constructor_Should_SnapshotErrors_When_InputCollectionChangesLater()
    {
        var errors = new List<string> { "Original error" };
        var exception = new CustomException("Request failed", errors, HttpStatusCode.BadRequest);

        errors.Add("Later error");

        Assert.Equal("Request failed", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal(["Original error"], exception.ErrorMessages);
    }

    [Fact]
    public void Constructor_Should_PreserveCauseAndStatus_When_InnerExceptionIsProvided()
    {
        var cause = new InvalidOperationException("Cause");

        var exception = new CustomException("Failure", cause, HttpStatusCode.Conflict);

        Assert.Same(cause, exception.InnerException);
        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        Assert.Empty(exception.ErrorMessages);
    }
}

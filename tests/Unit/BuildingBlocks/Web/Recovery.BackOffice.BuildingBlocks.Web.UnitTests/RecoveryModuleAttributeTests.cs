using Recovery.BackOffice.BuildingBlocks.Web.Modules;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Web.UnitTests;

public sealed class RecoveryModuleAttributeTests
{
    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_When_ModuleTypeIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new RecoveryModuleAttribute(null!));

        Assert.Equal("moduleType", exception.ParamName);
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(IModule))]
    [InlineData(typeof(TestModule))]
    [InlineData(typeof(ModuleWithDependency))]
    public void Constructor_Should_RejectRegistration_When_TypeCannotBeCreatedAsModule(Type moduleType)
    {
        var exception = Assert.Throws<ArgumentException>(() => new RecoveryModuleAttribute(moduleType));

        Assert.Equal("moduleType", exception.ParamName);
    }
}

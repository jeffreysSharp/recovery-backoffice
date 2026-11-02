using Recovery.BackOffice.Modules.Catalog.Domain;
namespace Recovery.BackOffice.Catalog.Tests.Domain;
public sealed class CityTests
{
    [Fact]
    public void Create_NormalizesUnicodeAndWhitespace_PreservesLeadingZeros()
    {
        var stateId = Guid.NewGuid();
        var city = City.Create(stateId, "  Sa\u0303o Paulo  ", " 0000001 ");
        city.Name.ShouldBe("São Paulo");
        city.StateId.ShouldBe(stateId);
        city.IbgeCode.ShouldBe("0000001");
        city.Id.Version.ShouldBe(7);
        city.IsActive.ShouldBeTrue();
        City.Create(stateId, "Exterior", " ").IbgeCode.ShouldBeNull();
    }
    [Theory]
    [InlineData("", null)]
    [InlineData("A", null)]
    [InlineData("Cidade", "123456")]
    [InlineData("Cidade", "12345678")]
    [InlineData("Cidade", "12345AB")]
    public void Create_RejectsInvalidFields(string name, string? ibgeCode) =>
        Should.Throw<ArgumentException>(() => City.Create(Guid.NewGuid(), name, ibgeCode));
    [Fact]
    public void Update_PreservesId_AndChangesStateAndStatus()
    {
        var city = City.Create(Guid.NewGuid(), "Cidade", "1234567");
        var id = city.Id;
        var stateId = Guid.NewGuid();
        city.Update(stateId, " Atualizada ", null, false);
        city.Id.ShouldBe(id);
        city.StateId.ShouldBe(stateId);
        city.Name.ShouldBe("Atualizada");
        city.IsActive.ShouldBeFalse();
        city.Update(stateId, city.Name, null, true);
        city.IsActive.ShouldBeTrue();
        Should.Throw<ArgumentException>(() => City.Create(Guid.Empty, "Cidade"));
        Should.Throw<ArgumentException>(() => City.Create(stateId, new string('x',151)));
        City.Create(stateId, new string('x',150)).Name.Length.ShouldBe(150);
    }
}

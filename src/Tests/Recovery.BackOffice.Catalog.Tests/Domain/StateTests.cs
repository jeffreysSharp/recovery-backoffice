using Recovery.BackOffice.Modules.Catalog.Domain;
namespace Recovery.BackOffice.Catalog.Tests.Domain;
public sealed class StateTests
{
    [Fact]
    public void Create_NormalizesWithoutForcingBrazilianCodeLength()
    {
        var countryId = Guid.NewGuid();
        var state = State.Create(countryId, "  São Paulo  ", " sp ", " 35 ");
        state.CountryId.ShouldBe(countryId);
        state.Name.ShouldBe("São Paulo");
        state.Code.ShouldBe("SP");
        state.IbgeCode.ShouldBe("35");
        state.Id.Version.ShouldBe(7);
        state.IsActive.ShouldBeTrue();
        State.Create(countryId, "Exterior", "abc-123").Code.ShouldBe("ABC-123");
        State.Create(countryId, "Exterior", "a", " ").IbgeCode.ShouldBeNull();
    }
    [Theory]
    [InlineData("", "SP", null)]
    [InlineData("A", "SP", null)]
    [InlineData("São Paulo", "", null)]
    [InlineData("São Paulo", "ABCDEFGHIJK", null)]
    [InlineData("São Paulo", "SP", "1")]
    [InlineData("São Paulo", "SP", "123")]
    [InlineData("São Paulo", "SP", "AB")]
    public void Create_RejectsInvalidFields(string name, string code, string? ibgeCode) =>
        Should.Throw<ArgumentException>(() => State.Create(Guid.NewGuid(), name, code, ibgeCode));
    [Fact]
    public void Update_PreservesIdAndAllowsActivation()
    {
        var state = State.Create(Guid.NewGuid(), "São Paulo", "SP", "35");
        var id = state.Id;
        var countryId = Guid.NewGuid();
        state.Update(countryId, "  Exterior ", " long-code ", null, false);
        state.Id.ShouldBe(id);
        state.CountryId.ShouldBe(countryId);
        state.Code.ShouldBe("LONG-CODE");
        state.IsActive.ShouldBeFalse();
        state.Update(countryId, state.Name, state.Code, null, true);
        state.IsActive.ShouldBeTrue();
        Should.Throw<ArgumentException>(() => State.Create(Guid.Empty, "Exterior", "XX"));
        Should.Throw<ArgumentException>(() => State.Create(countryId, new string('x', 101), "XX"));
    }
}

using Recovery.BackOffice.Modules.Catalog.Domain;
namespace Recovery.BackOffice.Catalog.Tests.Domain;
public sealed class CountryTests
{
    [Fact]
    public void Create_NormalizesCodes_AndPreservesLeadingZeros()
    {
        var country = Country.Create("  Brasil ", " br ", " bra ", " 076 ", " +55 ");
        country.Id.ShouldNotBe(Guid.Empty);
        country.Name.ShouldBe("Brasil");
        country.Iso2Code.ShouldBe("BR");
        country.Iso3Code.ShouldBe("BRA");
        country.NumericCode.ShouldBe("076");
        country.PhoneCode.ShouldBe("+55");
        country.IsActive.ShouldBeTrue();
    }
    [Theory]
    [InlineData("", "BR", "BRA", null, null)]
    [InlineData(" ", "BR", "BRA", null, null)]
    [InlineData("Brasil", "", "BRA", null, null)]
    [InlineData("Brasil", "B1", "BRA", null, null)]
    [InlineData("Brasil", "B", "BRA", null, null)]
    [InlineData("Brasil", "BR", "BR", null, null)]
    [InlineData("Brasil", "BR", "B1A", null, null)]
    [InlineData("Brasil", "BR", "BRA", "76", null)]
    [InlineData("Brasil", "BR", "BRA", "a76", null)]
    [InlineData("Brasil", "BR", "BRA", null, "55")]
    [InlineData("Brasil", "BR", "BRA", null, "+055")]
    public void Create_RejectsInvalidValues(string name, string iso2, string iso3, string? numeric, string? phone) =>
        Should.Throw<ArgumentException>(() => Country.Create(name, iso2, iso3, numeric, phone));

    [Fact]
    public void Create_RejectsLongName() =>
        Should.Throw<ArgumentException>(() => Country.Create(new string('x',151), "BR","BRA"));

    [Fact]
    public void Update_ChangesValuesAndAllowsActivation()
    {
        var country = Country.Create("Brasil","BR","BRA");
        country.Update("Portugal","pt","prt","620","+351",false);
        country.Name.ShouldBe("Portugal");
        country.Iso2Code.ShouldBe("PT");
        country.IsActive.ShouldBeFalse();
        country.Update("Portugal","PT","PRT",null,null,true);
        country.IsActive.ShouldBeTrue();
        country.NumericCode.ShouldBeNull();
    }
}

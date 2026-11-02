using System.Security.Cryptography;
using System.Text;
using Recovery.BackOffice.Modules.Catalog.Data;

namespace Recovery.BackOffice.Catalog.Tests.Data;

public sealed class CountrySeedDataTests
{
    [Fact]
    public void CompiledData_MatchesTheReviewedReferenceSnapshot()
    {
        var countries = CountrySeedData.BuildCountries();
        countries.Count.ShouldBe(249);
        countries.Select(country => country.Iso2Code).Distinct(StringComparer.Ordinal).Count().ShouldBe(249);
        countries.Select(country => country.Iso3Code).Distinct(StringComparer.Ordinal).Count().ShouldBe(249);
        countries.All(country => country.IsActive).ShouldBeTrue();

        // Fingerprint of the reviewed CLDR 49 / pinned phone metadata, independent of generated Ids.
        var canonical = string.Join("\n", countries.OrderBy(country => country.Iso2Code, StringComparer.Ordinal)
            .Select(country => string.Join("|", country.Name, country.Iso2Code, country.Iso3Code,
                country.NumericCode ?? string.Empty, country.PhoneCode ?? string.Empty, country.IsActive ? "true" : "false"))) + "\n";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
        hash.ShouldBe("E7DD5F284EAC3589D53E21C55825F9C83B0F7C4827ABC368D1FA7C81BF7064A9");
    }

    [Fact]
    public void NamesAndNumericCodes_PreservePortugueseAndLeadingZeroes()
    {
        var countries = CountrySeedData.BuildCountries().ToDictionary(country => country.Iso2Code, StringComparer.Ordinal);
        countries["BR"].Name.ShouldBe("Brasil");
        countries["BR"].Iso3Code.ShouldBe("BRA");
        countries["BR"].NumericCode.ShouldBe("076");
        countries["BR"].PhoneCode.ShouldBe("+55");
        countries["AF"].NumericCode.ShouldBe("004");
        countries["DE"].Name.ShouldBe("Alemanha");
        countries["ST"].Name.ShouldBe("São Tomé e Príncipe");
        countries["CI"].Name.ShouldBe("Costa do Marfim");
    }

    [Fact]
    public void PhoneCodes_ContainOnlyDdi_AndLeaveUnresolvedTerritoriesNull()
    {
        var countries = CountrySeedData.BuildCountries().ToDictionary(country => country.Iso2Code, StringComparer.Ordinal);
        countries["AG"].PhoneCode.ShouldBe("+1");
        countries["DO"].PhoneCode.ShouldBe("+1");
        countries["GG"].PhoneCode.ShouldBe("+44");
        countries["IM"].PhoneCode.ShouldBe("+44");
        countries["JE"].PhoneCode.ShouldBe("+44");
        countries["AX"].PhoneCode.ShouldBe("+358");
        countries["PN"].PhoneCode.ShouldBe("+64");
        var missing = countries.Values.Where(country => country.PhoneCode is null)
            .Select(country => country.Iso2Code).OrderBy(code => code, StringComparer.Ordinal).ToArray();
        missing.ShouldBe(new[] { "AQ", "BV", "GS", "HM", "SH", "TF", "UM" });
    }
}

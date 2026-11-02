using System.Security.Cryptography;
using System.Text;
using Recovery.BackOffice.Modules.Catalog.Data;
namespace Recovery.BackOffice.Catalog.Tests.Data;
public sealed class CitySeedDataTests
{
    [Fact]
    public void CompiledSnapshot_MatchesVerifiedOfficialResponse_AndAll27Ufs()
    {
        var rows = CitySeedData.BuildRecords();
        rows.Count.ShouldBe(5571); // Measured in the official response retrieved 2026-11-01, not assumed.
        rows.Select(row => row.IbgeCode).Distinct(StringComparer.Ordinal).Count().ShouldBe(rows.Count);
        var expectedUfs = StateSeedData.BuildBrazilStates(Guid.NewGuid()).Select(state => state.IbgeCode!).ToHashSet(StringComparer.Ordinal);
        rows.Select(row => row.StateIbgeCode).ToHashSet(StringComparer.Ordinal).SetEquals(expectedUfs).ShouldBeTrue();
        foreach (var row in rows)
        {
            row.IbgeCode.Length.ShouldBe(7);
            row.IbgeCode.All(char.IsAsciiDigit).ShouldBeTrue();
            row.IbgeCode.ShouldStartWith(row.StateIbgeCode);
        }
        var canonical = string.Concat(rows.OrderBy(row => row.IbgeCode, StringComparer.Ordinal)
            .Select(row => row.IbgeCode + "|" + row.StateIbgeCode + "|" + row.Name + "\n"));
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ShouldBe(CitySeedData.SnapshotSha256);
        CitySeedData.SnapshotSha256.ShouldBe("911F4999C40DA68DAC37031477D5D5695115B70DBA472E582C9D07B6ED5D915C");
        CitySeedData.ReferenceDate.ShouldBe("2026-11-01");
    }
    [Theory]
    [InlineData("3550308","São Paulo","35")]
    [InlineData("3513801","Diadema","35")]
    [InlineData("5300108","Brasília","53")]
    public void ReferenceCities_HaveCorrectNamesCodesAndUfs(string code, string name, string uf)
    {
        var row = CitySeedData.BuildRecords().Single(row => row.IbgeCode == code);
        row.Name.ShouldBe(name);
        row.StateIbgeCode.ShouldBe(uf);
    }
}

using Recovery.BackOffice.Modules.Catalog.Data;
namespace Recovery.BackOffice.Catalog.Tests.Data;
public sealed class StateSeedDataTests
{
    [Fact]
    public void BrazilReference_Has27UniqueActiveUfs_WithDynamicCountryId()
    {
        var countryId = Guid.NewGuid();
        var states = StateSeedData.BuildBrazilStates(countryId);
        states.Count.ShouldBe(27);
        states.All(state => state.CountryId == countryId && state.IsActive).ShouldBeTrue();
        states.Select(state => state.Code).Distinct(StringComparer.Ordinal).Count().ShouldBe(27);
        states.Select(state => state.IbgeCode).Distinct(StringComparer.Ordinal).Count().ShouldBe(27);
        states.Select(state => state.Id).Distinct().Count().ShouldBe(27);
        var expected = "AC:12;AL:27;AP:16;AM:13;BA:29;CE:23;DF:53;ES:32;GO:52;MA:21;MT:51;MS:50;MG:31;PA:15;PB:25;PR:41;PE:26;PI:22;RJ:33;RN:24;RS:43;RO:11;RR:14;SC:42;SP:35;SE:28;TO:17";
        string.Join(";", states.Select(state => state.Code + ":" + state.IbgeCode)).ShouldBe(expected);
        states.Single(state => state.Code == "SP").Name.ShouldBe("São Paulo");
        states.Single(state => state.Code == "DF").Name.ShouldBe("Distrito Federal");
        states.Single(state => state.Code == "ES").Name.ShouldBe("Espírito Santo");
        var second = StateSeedData.BuildBrazilStates(Guid.NewGuid());
        second.Select(state => state.Id).Intersect(states.Select(state => state.Id)).ShouldBeEmpty();
    }
}

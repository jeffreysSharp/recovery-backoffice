using Recovery.BackOffice.Modules.Catalog.Data;
namespace Recovery.BackOffice.Catalog.Tests.Data;
public sealed class AgencyTypeSeedDataTests
{
    [Fact]
    public void CompiledSeed_ContainsExactlyFourRequestedActiveTypes_WithoutFixedIds()
    {
        var rows = AgencyTypeSeedData.BuildAgencyTypes();
        var expected = new Dictionary<string,string>(StringComparer.Ordinal)
        {
            ["Escritório de Cobrança"]="Empresa especializada na recuperação e negociação de créditos.",
            ["Escritório Jurídico"]="Escritório especializado na cobrança judicial e recuperação de créditos por meios legais.",
            ["Escritório de Cobrança e Jurídico"]="Empresa que realiza cobrança extrajudicial e judicial.",
            ["Operação Interna"]="Equipe interna responsável pelas atividades de cobrança e recuperação de créditos."
        };
        rows.Count.ShouldBe(4);
        rows.Select(row=>row.Name).ToHashSet(StringComparer.Ordinal).SetEquals(expected.Keys).ShouldBeTrue();
        foreach(var row in rows)
        {
            row.IsActive.ShouldBeTrue();
            row.Description.ShouldBe(expected[row.Name]);
            row.Id.Version.ShouldBe(7);
        }
        rows.Select(row=>row.Id).Distinct().Count().ShouldBe(4);
        AgencyTypeSeedData.BuildAgencyTypes().Select(row=>row.Id).Intersect(rows.Select(row=>row.Id)).ShouldBeEmpty();
    }
}

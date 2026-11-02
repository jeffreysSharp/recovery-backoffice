using Recovery.BackOffice.Modules.Catalog.Domain;
namespace Recovery.BackOffice.Catalog.Tests.Domain;
public sealed class AgencyTypeTests
{
    [Fact]
    public void Create_NormalizesExcessWhitespaceAndUnicode_WithDefaultActive()
    {
        var agencyType = AgencyType.Create("  Operac\u0327a\u0303o\t  Interna\r\n ", "  Descrição administrativa  ");
        agencyType.Name.ShouldBe("Operação Interna");
        agencyType.Description.ShouldBe("Descrição administrativa");
        agencyType.IsActive.ShouldBeTrue();
        agencyType.Id.Version.ShouldBe(7);
        AgencyType.Create("Externo", " \t ").Description.ShouldBeNull();
    }
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("AB")]
    [InlineData("  A  ")]
    public void Create_RejectsMissingOrShortNames(string name) =>
        Should.Throw<ArgumentException>(() => AgencyType.Create(name));
    [Fact]
    public void Create_ValidatesNameAndDescriptionBoundaries()
    {
        AgencyType.Create("ABC").Name.ShouldBe("ABC");
        AgencyType.Create(new string('x',100),new string('d',500)).Description!.Length.ShouldBe(500);
        Should.Throw<ArgumentException>(() => AgencyType.Create(new string('x',101)));
        Should.Throw<ArgumentException>(() => AgencyType.Create("Válido",new string('d',501)));
    }
    [Fact]
    public void Update_PreservesId_ChangesDescription_AndAllowsActivation()
    {
        var agencyType = AgencyType.Create("Original","Inicial");
        var id = agencyType.Id;
        agencyType.Update("  Tipo\t Atualizado  ",null,false);
        agencyType.Id.ShouldBe(id);
        agencyType.Name.ShouldBe("Tipo Atualizado");
        agencyType.Description.ShouldBeNull();
        agencyType.IsActive.ShouldBeFalse();
        agencyType.Update(agencyType.Name,"Nova descrição",true);
        agencyType.IsActive.ShouldBeTrue();
    }
}

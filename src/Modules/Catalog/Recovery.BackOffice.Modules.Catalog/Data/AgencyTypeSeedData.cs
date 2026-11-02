using Recovery.BackOffice.Modules.Catalog.Domain;
namespace Recovery.BackOffice.Modules.Catalog.Data;
public static class AgencyTypeSeedData
{
    public static IReadOnlyList<AgencyType> BuildAgencyTypes() =>
    [
        AgencyType.Create("Escritório de Cobrança", "Empresa especializada na recuperação e negociação de créditos."),
        AgencyType.Create("Escritório Jurídico", "Escritório especializado na cobrança judicial e recuperação de créditos por meios legais."),
        AgencyType.Create("Escritório de Cobrança e Jurídico", "Empresa que realiza cobrança extrajudicial e judicial."),
        AgencyType.Create("Operação Interna", "Equipe interna responsável pelas atividades de cobrança e recuperação de créditos."),
    ];
}

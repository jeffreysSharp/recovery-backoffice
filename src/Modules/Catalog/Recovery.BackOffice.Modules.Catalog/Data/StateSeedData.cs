using Recovery.BackOffice.Modules.Catalog.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Data;

/// <summary>Official Brazilian UF names/codes provided for this catalog; fresh Ids, dynamic CountryId.</summary>
public static class StateSeedData
{
    public static IReadOnlyList<State> BuildBrazilStates(Guid countryId) =>
    [
        State.Create(countryId, "Acre", "AC", "12", true),
        State.Create(countryId, "Alagoas", "AL", "27", true),
        State.Create(countryId, "Amapá", "AP", "16", true),
        State.Create(countryId, "Amazonas", "AM", "13", true),
        State.Create(countryId, "Bahia", "BA", "29", true),
        State.Create(countryId, "Ceará", "CE", "23", true),
        State.Create(countryId, "Distrito Federal", "DF", "53", true),
        State.Create(countryId, "Espírito Santo", "ES", "32", true),
        State.Create(countryId, "Goiás", "GO", "52", true),
        State.Create(countryId, "Maranhão", "MA", "21", true),
        State.Create(countryId, "Mato Grosso", "MT", "51", true),
        State.Create(countryId, "Mato Grosso do Sul", "MS", "50", true),
        State.Create(countryId, "Minas Gerais", "MG", "31", true),
        State.Create(countryId, "Pará", "PA", "15", true),
        State.Create(countryId, "Paraíba", "PB", "25", true),
        State.Create(countryId, "Paraná", "PR", "41", true),
        State.Create(countryId, "Pernambuco", "PE", "26", true),
        State.Create(countryId, "Piauí", "PI", "22", true),
        State.Create(countryId, "Rio de Janeiro", "RJ", "33", true),
        State.Create(countryId, "Rio Grande do Norte", "RN", "24", true),
        State.Create(countryId, "Rio Grande do Sul", "RS", "43", true),
        State.Create(countryId, "Rondônia", "RO", "11", true),
        State.Create(countryId, "Roraima", "RR", "14", true),
        State.Create(countryId, "Santa Catarina", "SC", "42", true),
        State.Create(countryId, "São Paulo", "SP", "35", true),
        State.Create(countryId, "Sergipe", "SE", "28", true),
        State.Create(countryId, "Tocantins", "TO", "17", true),
    ];
}

using System.Text;
using System.Text.RegularExpressions;
using Recovery.BackOffice.BuildingBlocks.Core.Domain;
namespace Recovery.BackOffice.Modules.Catalog.Domain;

public sealed class City : AggregateRoot<Guid>, IGlobalEntity, IAuditableEntity
{
    public Guid StateId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? IbgeCode { get; private set; }
    public bool IsActive { get; private set; } = true;
#pragma warning disable S1144 // EF Core populates audit metadata.
    public DateTimeOffset CreatedOnUtc { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedOnUtc { get; private set; }
    public string? LastModifiedBy { get; private set; }
#pragma warning restore S1144
    private City() { }
    public static City Create(Guid stateId, string name, string? ibgeCode = null, bool isActive = true)
    {
        var city = new City { Id = Guid.CreateVersion7() };
        city.Update(stateId, name, ibgeCode, isActive);
        return city;
    }
    internal static string NormalizeName(string name) => name.Trim().Normalize(NormalizationForm.FormC);
    public void Update(Guid stateId, string name, string? ibgeCode, bool isActive)
    {
        if (stateId == Guid.Empty) throw new ArgumentException("Estado é obrigatório.", nameof(stateId));
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var normalized = NormalizeName(name);
        var ibge = string.IsNullOrWhiteSpace(ibgeCode) ? null : ibgeCode.Trim();
        if (normalized.Length is < 2 or > 150)
            throw new ArgumentException("Cidade deve ter entre 2 e 150 caracteres.", nameof(name));
        if (ibge is not null && !Regex.IsMatch(ibge, "^[0-9]{7}$", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)))
            throw new ArgumentException("Código IBGE deve conter exatamente 7 dígitos.", nameof(ibgeCode));
        StateId = stateId;
        Name = normalized;
        IbgeCode = ibge;
        IsActive = isActive;
    }
}

using System.Text.RegularExpressions;
using Recovery.BackOffice.BuildingBlocks.Core.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Domain;

public sealed class State : AggregateRoot<Guid>, IGlobalEntity, IAuditableEntity
{
    public Guid CountryId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public string? IbgeCode { get; private set; }
    public bool IsActive { get; private set; } = true;
#pragma warning disable S1144 // EF Core populates the audit metadata.
    public DateTimeOffset CreatedOnUtc { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedOnUtc { get; private set; }
    public string? LastModifiedBy { get; private set; }
#pragma warning restore S1144

    private State() { }

    public static State Create(Guid countryId, string name, string code, string? ibgeCode = null, bool isActive = true)
    {
        var state = new State { Id = Guid.CreateVersion7() };
        state.Update(countryId, name, code, ibgeCode, isActive);
        return state;
    }

    public void Update(Guid countryId, string name, string code, string? ibgeCode, bool isActive)
    {
        if (countryId == Guid.Empty) throw new ArgumentException("País é obrigatório.", nameof(countryId));
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        var normalizedName = name.Trim();
        var normalizedCode = code.Trim().ToUpperInvariant();
        var ibge = string.IsNullOrWhiteSpace(ibgeCode) ? null : ibgeCode.Trim();
        if (normalizedName.Length is < 2 or > 100)
            throw new ArgumentException("Estado deve ter entre 2 e 100 caracteres.", nameof(name));
        if (normalizedCode.Length is < 1 or > 10)
            throw new ArgumentException("Sigla deve ter entre 1 e 10 caracteres.", nameof(code));
        if (ibge is not null && !Regex.IsMatch(ibge, "^[0-9]{2}$", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)))
            throw new ArgumentException("Código IBGE deve conter exatamente 2 dígitos.", nameof(ibgeCode));
        CountryId = countryId;
        Name = normalizedName;
        Code = normalizedCode;
        IbgeCode = ibge;
        IsActive = isActive;
    }
}

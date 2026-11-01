using System.Text.RegularExpressions;
using Recovery.BackOffice.BuildingBlocks.Core.Domain;

namespace Recovery.BackOffice.Modules.Catalog.Domain;

public sealed class Country : AggregateRoot<Guid>, IGlobalEntity, IAuditableEntity
{
    public string Name { get; private set; } = default!;
    public string Iso2Code { get; private set; } = default!;
    public string Iso3Code { get; private set; } = default!;
    public string? NumericCode { get; private set; }
    public string? PhoneCode { get; private set; }
    public bool IsActive { get; private set; } = true;
    #pragma warning disable S1144 // Set by EF Core through AuditableEntitySaveChangesInterceptor.
    public DateTimeOffset CreatedOnUtc { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedOnUtc { get; private set; }
    public string? LastModifiedBy { get; private set; }
    #pragma warning restore S1144
    private Country() { }
    public static Country Create(string name, string iso2Code, string iso3Code,
        string? numericCode = null, string? phoneCode = null, bool isActive = true)
    {
        var country = new Country { Id = Guid.CreateVersion7() };
        country.Update(name, iso2Code, iso3Code, numericCode, phoneCode, isActive);
        return country;
    }
    public void Update(string name, string iso2Code, string iso3Code, string? numericCode, string? phoneCode, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var normalizedName = name.Trim();
        var iso2 = (iso2Code ?? string.Empty).Trim().ToUpperInvariant();
        var iso3 = (iso3Code ?? string.Empty).Trim().ToUpperInvariant();
        var numeric = string.IsNullOrWhiteSpace(numericCode) ? null : numericCode.Trim();
        var phone = string.IsNullOrWhiteSpace(phoneCode) ? null : phoneCode.Trim();
        if (normalizedName.Length > 150) throw new ArgumentException("País deve ter no máximo 150 caracteres.", nameof(name));
        if (!Regex.IsMatch(iso2, "^[A-Z]{2}$", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)))
            throw new ArgumentException("Código ISO 2 deve conter exatamente 2 letras.", nameof(iso2Code));
        if (!Regex.IsMatch(iso3, "^[A-Z]{3}$", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)))
            throw new ArgumentException("Código ISO 3 deve conter exatamente 3 letras.", nameof(iso3Code));
        if (numeric is not null && !Regex.IsMatch(numeric, "^[0-9]{3}$", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)))
            throw new ArgumentException("Código numérico deve conter exatamente 3 dígitos.", nameof(numericCode));
        if (phone is not null && !Regex.IsMatch(phone, @"^\+[1-9][0-9]{0,3}$", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)))
            throw new ArgumentException("Código telefônico deve conter + e de 1 a 4 dígitos.", nameof(phoneCode));
        Name = normalizedName;
        Iso2Code = iso2;
        Iso3Code = iso3;
        NumericCode = numeric;
        PhoneCode = phone;
        IsActive = isActive;
    }
}

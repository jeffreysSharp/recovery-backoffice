using System.Text;
using System.Text.RegularExpressions;
using Recovery.BackOffice.BuildingBlocks.Core.Domain;
namespace Recovery.BackOffice.Modules.Catalog.Domain;
public sealed class AgencyType : AggregateRoot<Guid>, IGlobalEntity, IAuditableEntity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
#pragma warning disable S1144 // EF Core populates audit metadata.
    public DateTimeOffset CreatedOnUtc { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedOnUtc { get; private set; }
    public string? LastModifiedBy { get; private set; }
#pragma warning restore S1144
    private AgencyType() { }
    internal static string NormalizeName(string name) => Regex.Replace(name.Trim().Normalize(NormalizationForm.FormC),
        @"\s+", " ", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
    public static AgencyType Create(string name, string? description = null, bool isActive = true)
    {
        var agencyType = new AgencyType { Id = Guid.CreateVersion7() };
        agencyType.Update(name, description, isActive);
        return agencyType;
    }
    public void Update(string name, string? description, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var normalizedName = NormalizeName(name);
        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalizedName.Length is < 3 or > 100)
            throw new ArgumentException("Tipo de agência deve ter entre 3 e 100 caracteres.", nameof(name));
        if (normalizedDescription?.Length > 500)
            throw new ArgumentException("Descrição deve ter no máximo 500 caracteres.", nameof(description));
        Name = normalizedName;
        Description = normalizedDescription;
        IsActive = isActive;
    }
}

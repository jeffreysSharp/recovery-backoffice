using System.ComponentModel.DataAnnotations;

namespace Recovery.BackOffice.BuildingBlocks.Shared.Persistence;

public sealed class DatabaseOptions : IValidatableObject
{
    public string Provider { get; set; } = DbProviders.MSSQL;

    public string ConnectionString { get; set; } = string.Empty;

    public string MigrationsAssembly { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            yield return new ValidationResult(
                "Connection string cannot be empty.", [nameof(ConnectionString)]);
        }
    }
}

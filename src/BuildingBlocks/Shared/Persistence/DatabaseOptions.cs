using System.ComponentModel.DataAnnotations;

namespace Recovery.BackOffice.BuildingBlocks.Shared.Persistence;

/// <summary>
/// Configuration options for database provider selection and connection information.
/// </summary>
public sealed class DatabaseOptions : IValidatableObject
{
    /// <summary>
    /// The database provider to use. Valid values are <see cref="DbProviders.MSSQL"/>.
    /// Defaults to SQL Server.
    /// </summary>
    public string Provider { get; set; } = DbProviders.MSSQL;

    /// <summary>
    /// The connection string used by EF Core DbContexts and related services.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// The assembly that contains EF Core migrations for the selected provider.
    /// </summary>
    public string MigrationsAssembly { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            yield return new ValidationResult("connection string cannot be empty.", new[] { nameof(ConnectionString) });
        }
    }
}
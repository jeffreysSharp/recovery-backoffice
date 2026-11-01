using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Recovery.BackOffice.BuildingBlocks.Persistence;

/// <summary>Preserves UTC semantics when SQL Server materializes datetime2 values without a Kind.</summary>
public sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    value => value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc),
    value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

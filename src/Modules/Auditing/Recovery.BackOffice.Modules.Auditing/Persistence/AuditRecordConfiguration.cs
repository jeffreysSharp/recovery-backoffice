using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Recovery.BackOffice.Modules.Auditing.Persistence;

public class AuditRecordConfiguration : IEntityTypeConfiguration<AuditRecord>
{
    public void Configure(EntityTypeBuilder<AuditRecord> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("AuditRecords", "audit", t => t.HasCheckConstraint("CK_AuditRecords_Json", "ISJSON([PayloadJson]) = 1"));
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).HasConversion<int>();
        builder.Property(x => x.Severity).HasConversion<byte>();
        builder.Property(x => x.Tags).HasConversion<long>();
        builder.Property(x => x.PayloadJson).HasColumnType("nvarchar(max)");

        // Hot-path index: default audits list filters on TenantId (Finbuckle) and orders by OccurredAtUtc DESC.
        // A composite over both lets SQL Server serve the paged top-N from an index-only walk.
        builder.HasIndex(x => new { x.TenantId, x.OccurredAtUtc })
            .IsDescending(false, true)
            .HasDatabaseName("IX_AuditRecords_Tenant_OccurredAt");

        // Common dashboard slice: EventType within a tenant, ordered by time. Beats the
        // (TenantId, OccurredAtUtc) index when EventType is selective (e.g. only Security events).
        builder.HasIndex(x => new { x.TenantId, x.EventType, x.OccurredAtUtc })
            .IsDescending(false, false, true)
            .HasDatabaseName("IX_AuditRecords_Tenant_EventType_OccurredAt");

        // Trace/correlation lookups ("everything tied to this request"). Both columns are sparse,
        // so single-column indexes are cheap and frequently selective.
        builder.HasIndex(x => x.CorrelationId)
            .HasDatabaseName("IX_AuditRecords_CorrelationId");
        builder.HasIndex(x => x.TraceId)
            .HasDatabaseName("IX_AuditRecords_TraceId");

    }
}

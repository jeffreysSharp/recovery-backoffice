using Recovery.BackOffice.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Recovery.BackOffice.Modules.Catalog.Data.Configurations;

public sealed class StateConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("States", table =>
        {
            table.HasCheckConstraint("CK_States_Name", "LEN(LTRIM(RTRIM([Name]))) BETWEEN 2 AND 100");
            table.HasCheckConstraint("CK_States_Code", "LEN(LTRIM(RTRIM([Code]))) BETWEEN 1 AND 10");
            table.HasCheckConstraint("CK_States_IbgeCode", "[IbgeCode] IS NULL OR (LEN([IbgeCode]) = 2 AND [IbgeCode] NOT LIKE '%[^0-9]%')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(10).IsRequired();
        builder.Property(x => x.IbgeCode).HasMaxLength(2);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.HasOne<Country>().WithMany().HasForeignKey(x => x.CountryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CountryId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.CountryId, x.IbgeCode }).IsUnique().HasFilter("[IbgeCode] IS NOT NULL");
        builder.Property(x => x.CreatedBy).HasMaxLength(64);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(64);
        builder.Ignore(x => x.DomainEvents);
    }
}

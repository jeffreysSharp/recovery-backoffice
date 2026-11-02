using Recovery.BackOffice.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Recovery.BackOffice.Modules.Catalog.Data.Configurations;
public sealed class AgencyTypeConfiguration : IEntityTypeConfiguration<AgencyType>
{
    public void Configure(EntityTypeBuilder<AgencyType> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("AgencyTypes", table =>
            table.HasCheckConstraint("CK_AgencyTypes_Name", "LEN(LTRIM(RTRIM([Name]))) BETWEEN 3 AND 100"));
        builder.HasKey(agencyType => agencyType.Id);
        builder.Property(agencyType => agencyType.Name).HasMaxLength(100).UseCollation("Latin1_General_100_CI_AS").IsRequired();
        builder.Property(agencyType => agencyType.Description).HasMaxLength(500);
        builder.Property(agencyType => agencyType.IsActive).HasDefaultValue(true);
        builder.HasIndex(agencyType => agencyType.Name).IsUnique();
        builder.Property(agencyType => agencyType.CreatedBy).HasMaxLength(64);
        builder.Property(agencyType => agencyType.LastModifiedBy).HasMaxLength(64);
        builder.Ignore(agencyType => agencyType.DomainEvents);
    }
}

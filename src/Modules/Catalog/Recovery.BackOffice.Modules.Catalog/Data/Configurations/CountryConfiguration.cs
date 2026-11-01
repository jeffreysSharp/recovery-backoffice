using Recovery.BackOffice.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Recovery.BackOffice.Modules.Catalog.Data.Configurations;

public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Countries", table =>
        {
            table.HasCheckConstraint("CK_Countries_Iso2", "LEN([Iso2Code]) = 2 AND [Iso2Code] COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^A-Z]%'");
            table.HasCheckConstraint("CK_Countries_Iso3", "LEN([Iso3Code]) = 3 AND [Iso3Code] COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^A-Z]%'");
            table.HasCheckConstraint("CK_Countries_Name", "LEN(LTRIM(RTRIM([Name]))) > 0");
            table.HasCheckConstraint("CK_Countries_Numeric", "[NumericCode] IS NULL OR (LEN([NumericCode]) = 3 AND [NumericCode] NOT LIKE '%[^0-9]%')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Iso2Code).HasMaxLength(2).IsRequired();
        builder.Property(x => x.Iso3Code).HasMaxLength(3).IsRequired();
        builder.Property(x => x.NumericCode).HasMaxLength(3);
        builder.Property(x => x.PhoneCode).HasMaxLength(5);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.HasIndex(x => x.Iso2Code).IsUnique();
        builder.HasIndex(x => x.Iso3Code).IsUnique();
        builder.Property(x => x.CreatedBy).HasMaxLength(64);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(64);
        builder.Ignore(x => x.DomainEvents);
    }
}

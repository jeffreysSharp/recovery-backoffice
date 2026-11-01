using Recovery.BackOffice.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Recovery.BackOffice.Modules.Catalog.Data.Configurations;
public sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Cities", table =>
        {
            table.HasCheckConstraint("CK_Cities_Name", "LEN(LTRIM(RTRIM([Name]))) BETWEEN 2 AND 150");
            table.HasCheckConstraint("CK_Cities_IbgeCode", "[IbgeCode] IS NULL OR (LEN([IbgeCode]) = 7 AND [IbgeCode] NOT LIKE '%[^0-9]%')");
        });
        builder.HasKey(city => city.Id);
        builder.Property(city => city.Name).HasMaxLength(150).UseCollation("Latin1_General_100_CI_AI").IsRequired();
        builder.Property(city => city.IbgeCode).HasMaxLength(7);
        builder.Property(city => city.IsActive).HasDefaultValue(true);
        builder.HasOne<State>().WithMany().HasForeignKey(city => city.StateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(city => city.StateId);
        builder.HasIndex(city => new { city.StateId, city.Name }).IsUnique();
        builder.HasIndex(city => city.IbgeCode).IsUnique().HasFilter("[IbgeCode] IS NOT NULL");
        builder.Property(city => city.CreatedBy).HasMaxLength(64);
        builder.Property(city => city.LastModifiedBy).HasMaxLength(64);
        builder.Ignore(city => city.DomainEvents);
    }
}

using BookStation.Domain.Entities.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStation.Infrastructure.Data.Configurations;

public class SellerProfileConfiguration : IEntityTypeConfiguration<SellerProfile>
{
    public void Configure(EntityTypeBuilder<SellerProfile> builder)
    {
        builder.ToTable("SellerProfiles");

        builder.HasKey(sp => sp.Id);

        builder.HasOne(sp => sp.User)
            .WithOne()
            .HasForeignKey<SellerProfile>("UserId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property<Guid>("UserId").IsRequired();

        // Address - Owned Value Object
        builder.OwnsOne(sp => sp.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Address_Street")
                .HasMaxLength(500);

            address.Property(a => a.Ward)
                .HasColumnName("Address_Ward")
                .HasMaxLength(200);

            address.Property(a => a.City)
                .HasColumnName("Address_City")
                .HasMaxLength(200);

            address.Property(a => a.Country)
                .HasColumnName("Address_Country")
                .HasMaxLength(100);

            address.Property(a => a.PostalCode)
                .HasColumnName("Address_PostalCode")
                .HasMaxLength(20);
        });

        // Other properties
        builder.Property(sp => sp.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(sp => sp.OrganizationId);

        builder.Property(sp => sp.DateOfBirth);

        builder.Property(sp => sp.Gender)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(sp => sp.IdNumber)
            .HasMaxLength(50);

        builder.Property(sp => sp.ApprovedAt);

        builder.Property(sp => sp.CreatedAt)
            .IsRequired();

        builder.Property(sp => sp.UpdatedAt)
            .IsRequired();
    }
}
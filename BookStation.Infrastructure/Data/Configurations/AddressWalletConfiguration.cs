using BookStation.Domain.Entities.UserAggregate;
using BookStation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStation.Infrastructure.Data.Configurations;

public class AddressWalletConfiguration : IEntityTypeConfiguration<AddressWallet>
{
    public void Configure(EntityTypeBuilder<AddressWallet> builder)
    {
        builder.ToTable("AddressWallets");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.RecipientName)
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(a => a.RecipientPhone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("RecipientPhone")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.OwnsOne(a => a.RecipientAddress, address =>
        {
            address.Property(ad => ad.Street)
                .HasColumnName("Street")
                .HasMaxLength(500)
                .IsRequired();

            address.Property(ad => ad.Ward)
                .HasColumnName("Ward")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(ad => ad.City)
                .HasColumnName("City")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(ad => ad.Country)
                .HasColumnName("Country")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(ad => ad.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(20);
        });

        builder.Property(a => a.Label)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.IsDefault)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        // Relationship with User
        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for faster queries
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => new { a.UserId, a.IsDefault });
    }
}

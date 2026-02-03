using BookStation.Domain.Entities.UserAggregate;
using BookStation.Domain.Enums;
using BookStation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStation.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        // Email - Value Object
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(256)
                .IsRequired();

            email.HasIndex(e => e.Value)
                .IsUnique();
        });

        // PhoneNumber - Value Object
        builder.OwnsOne(u => u.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20);
        });

        // PasswordHash - Value Object
        builder.OwnsOne(u => u.PasswordHash, password =>
        {
            password.Property(p => p.HashedValue)
                .HasColumnName("PasswordHash")
                .HasMaxLength(256)
                .IsRequired();
        });

        // FullName
        builder.Property(u => u.FullName)
            .HasMaxLength(200)
            .IsRequired();

        // IsVerified
        builder.Property(u => u.IsVerified)
            .IsRequired();

        // Status
        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(500);
        
        // Timestamps
        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .IsRequired();

        // Relationship with SellerProfile (One-to-One)
        builder.HasOne(u => u.SellerProfile)
            .WithOne(sp => sp.User)
            .HasForeignKey<SellerProfile>(sp => sp.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events (not persisted)
        builder.Ignore(u => u.DomainEvents);
    }
}
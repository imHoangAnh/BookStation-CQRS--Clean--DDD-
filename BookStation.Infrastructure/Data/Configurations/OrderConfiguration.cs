using BookStation.Domain.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStation.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedOnAdd();

        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(o => o.Notes).HasMaxLength(2000);
        builder.Property(o => o.CancellationReason).HasMaxLength(1000);

        builder.OwnsOne(o => o.TotalAmount, m =>
        {
            m.Property(x => x.Amount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)").IsRequired();
            m.Property(x => x.Currency).HasColumnName("TotalAmountCurrency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(o => o.DiscountAmount, m =>
        {
            m.Property(x => x.Amount).HasColumnName("DiscountAmount").HasColumnType("decimal(18,2)").IsRequired();
            m.Property(x => x.Currency).HasColumnName("DiscountAmountCurrency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(o => o.FinalAmount, m =>
        {
            m.Property(x => x.Amount).HasColumnName("FinalAmount").HasColumnType("decimal(18,2)").IsRequired();
            m.Property(x => x.Currency).HasColumnName("FinalAmountCurrency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(o => o.ShippingAddress, a =>
        {
            a.Property(x => x.Street).HasColumnName("ShippingStreet").HasMaxLength(500).IsRequired();
            a.Property(x => x.Ward).HasColumnName("ShippingWard").HasMaxLength(100).IsRequired();
            a.Property(x => x.City).HasColumnName("ShippingCity").HasMaxLength(100).IsRequired();
            a.Property(x => x.Country).HasColumnName("ShippingCountry").HasMaxLength(100).IsRequired();
            a.Property(x => x.PostalCode).HasColumnName("ShippingPostalCode").HasMaxLength(20);
        });

        builder.HasMany(o => o.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(o => o.Payments).WithOne(p => p.Order).HasForeignKey(p => p.OrderId).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(o => o.DomainEvents);
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.Status);
    }
}

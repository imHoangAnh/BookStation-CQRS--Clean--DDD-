using BookStation.Domain.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStation.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id).ValueGeneratedOnAdd();

        builder.Property(oi => oi.BookTitle).HasMaxLength(500).IsRequired();
        builder.Property(oi => oi.VariantName).HasMaxLength(200).IsRequired();

        builder.OwnsOne(oi => oi.UnitPrice, m =>
        {
            m.Property(x => x.Amount).HasColumnName("UnitPrice").HasColumnType("decimal(18,2)").IsRequired();
            m.Property(x => x.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(3).IsRequired();
        });

        builder.Ignore(oi => oi.Subtotal);
        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.BookVariantId);
    }
}

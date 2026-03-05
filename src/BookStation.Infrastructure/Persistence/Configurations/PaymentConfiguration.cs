using BookStation.Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStation.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(p => p.TransactionId).HasMaxLength(200);

        builder.OwnsOne(p => p.Amount, m =>
        {
            m.Property(x => x.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)").IsRequired();
            m.Property(x => x.Currency).HasColumnName("AmountCurrency").HasMaxLength(3).IsRequired();
        });

        builder.HasIndex(p => p.OrderId);
    }
}

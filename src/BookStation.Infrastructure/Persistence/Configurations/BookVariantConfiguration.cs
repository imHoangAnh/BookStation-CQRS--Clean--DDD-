using BookStation.Core.Entities.BookAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStation.Infrastructure.Data.Configurations;

public class BookVariantConfiguration : IEntityTypeConfiguration<BookVariant>
{
    public void Configure(EntityTypeBuilder<BookVariant> builder)
    {
        builder.ToTable("BookVariants");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedOnAdd();

        builder.Property(v => v.VariantName).HasMaxLength(200).IsRequired();
        builder.Property(v => v.SKU).HasMaxLength(100);

        builder.OwnsOne(v => v.Price, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Price").HasColumnType("decimal(18,2)").IsRequired();
            money.Property(m => m.Currency).HasColumnName("PriceCurrency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(v => v.OriginalPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("OriginalPrice").HasColumnType("decimal(18,2)");
            money.Property(m => m.Currency).HasColumnName("OriginalPriceCurrency").HasMaxLength(3);
        });

        builder.HasOne(v => v.Inventory).WithOne(i => i.BookVariant).HasForeignKey<Inventory>(i => i.BookVariantId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => v.BookId);
        builder.HasIndex(v => v.SKU).IsUnique().HasFilter("[SKU] IS NOT NULL");
    }
}

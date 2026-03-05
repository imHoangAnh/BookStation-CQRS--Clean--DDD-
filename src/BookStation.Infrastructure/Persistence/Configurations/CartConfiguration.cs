using BookStation.Core.Entities.CartAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStation.Infrastructure.Data.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");
        builder.HasKey(c => c.Id);

        builder.HasMany(c => c.Items).WithOne().HasForeignKey("CartId").OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(c => c.DomainEvents);
        builder.HasIndex(c => c.UserId).IsUnique();
    }
}

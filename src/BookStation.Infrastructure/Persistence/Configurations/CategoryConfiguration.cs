using BookStation.Core.Entities.BookAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStation.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(1000);

        builder.HasOne(c => c.ParentCategory).WithMany().HasForeignKey(c => c.ParentCategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(c => c.BookCategories).WithOne(bc => bc.Category).HasForeignKey(bc => bc.CategoryId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.Name);
    }
}

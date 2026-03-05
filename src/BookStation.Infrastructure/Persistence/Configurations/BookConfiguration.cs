using BookStation.Core.Entities.BookAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStation.Infrastructure.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();

        builder.Property(b => b.Title).HasMaxLength(500).IsRequired();
        builder.Property(b => b.Description).HasMaxLength(4000);
        builder.Property(b => b.Language).HasMaxLength(50);
        builder.Property(b => b.CoverImageUrl).HasMaxLength(1000);
        builder.Property(b => b.CoAuthors).HasMaxLength(1000);
        builder.Property(b => b.Translators).HasMaxLength(1000);
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(50).IsRequired();

        builder.OwnsOne(b => b.ISBN, isbn =>
        {
            isbn.Property(i => i.Value).HasColumnName("ISBN").HasMaxLength(13);
            isbn.HasIndex(i => i.Value).IsUnique();
        });

        builder.HasOne(b => b.Publisher).WithMany(p => p.Books).HasForeignKey(b => b.PublisherId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(b => b.Variants).WithOne(v => v.Book).HasForeignKey(v => v.BookId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(b => b.BookAuthors).WithOne(ba => ba.Book).HasForeignKey(ba => ba.BookId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(b => b.BookCategories).WithOne(bc => bc.Book).HasForeignKey(bc => bc.BookId).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(b => b.DomainEvents);
        builder.HasIndex(b => b.PublisherId);
        builder.HasIndex(b => b.Status);
    }
}

using BookStation.Domain.Entities.BookAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStation.Infrastructure.Data.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FullName).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Bio).HasMaxLength(4000);
        builder.Property(a => a.Address).HasMaxLength(500);
        builder.Property(a => a.Country).HasMaxLength(100);
        builder.Property(a => a.PhotoUrl).HasMaxLength(1000);

        builder.HasIndex(a => a.UserId);
    }
}

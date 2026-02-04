using BookStation.Domain.Entities.UserAggregate;
using BookStation.Core.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Infrastructure.Data;

public class BookStationDbContext : DbContext
{
    public BookStationDbContext(DbContextOptions<BookStationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<AddressWallet> AddressWallets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookStationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set timestamps before saving
        var entries = ChangeTracker.Entries<Entity<Guid>>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

//1.	Khi gọi SaveChangesAsync()
//2.	Tự động set CreatedAt cho entities mới
//3.	Tự động set UpdatedAt cho tất cả entities thay đổi
//4.	Lưu vào database
//5.	Trả về số records đã lưu
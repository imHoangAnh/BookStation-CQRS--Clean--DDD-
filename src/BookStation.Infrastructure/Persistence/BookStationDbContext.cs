using BookStation.Core.Entities.UserAggregate;
using BookStation.Core.Entities.BookAggregate;
using BookStation.Core.Entities.CartAggregate;
using BookStation.Core.Entities.OrderAggregate;
using BookStation.Core.SharedKernel;
using BookStation.Query.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Infrastructure.Data;

public class BookStationDbContext : DbContext, IReadDbContext
{
    public BookStationDbContext(DbContextOptions<BookStationDbContext> options) : base(options) { }

    // UserAggregate
    public DbSet<User> Users => Set<User>();
    public DbSet<AddressWallet> AddressWallets => Set<AddressWallet>();

    // BookAggregate
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookVariant> BookVariants => Set<BookVariant>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
    public DbSet<BookCategory> BookCategories => Set<BookCategory>();

    // CartAggregate
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    // OrderAggregate
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookStationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Entity<Guid>>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

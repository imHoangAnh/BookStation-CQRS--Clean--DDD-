using Microsoft.EntityFrameworkCore;
using BookStation.Core.Entities.UserAggregate;
using BookStation.Core.Entities.OrderAggregate;
using BookStation.Core.Entities.BookAggregate;

namespace BookStation.Query.Abstractions;

/// <summary>
/// Read-only context for the read flow. Exposes only DbSet for querying; no SaveChanges.
/// </summary>
public interface IReadDbContext
{
    DbSet<Book> Books { get; }
    DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }
    DbSet<AddressWallet> AddressWallets { get; }
}

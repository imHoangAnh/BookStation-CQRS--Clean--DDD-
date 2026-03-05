using Microsoft.EntityFrameworkCore;
using BookStation.Core.Entities.UserAggregate;
using BookStation.Core.Entities.OrderAggregate;
using BookStation.Core.Entities.BookAggregate;

namespace BookStation.Application.Abstractions;

public interface IReadDbContext
{
    DbSet<Book> Books { get; }
    DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }
}
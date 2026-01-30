using BookStation.Domain.Entities;
using BookStation.Domain.Entities.UserAggregate;
using BookStation.Domain.Repositories;
using BookStation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BookStationDbContext _dbContext;

    public UserRepository(BookStationDbContext dbContext)           
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(User entity, CancellationToken cancellation = default)
    {
        await _dbContext.Users.AddAsync(entity, cancellation);
        await _dbContext.SaveChangesAsync(cancellation);
    }

    public async Task TaskAsync(User entity, CancellationToken cancellation = default)
    {
        await _dbContext.Users.AddAsync(entity, cancellation);
        await _dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellation = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellation);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellation = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email.ToLowerInvariant(), cancellation);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellation = default)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Email.Value == email.ToLowerInvariant(), cancellation);
    }

    public async Task<User?> GetWithRolesAsync(Guid id, CancellationToken cancellation = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellation);
    }

    public async Task<User?> GetWithSellerProfileAsync(Guid id, CancellationToken cancellation = default)
    {
        return await _dbContext.Users
            .Include(u => u.SellerProfile)
            .FirstOrDefaultAsync(u => u.Id == id, cancellation);
    }

    public void Update(User entity)
    {
        _dbContext.Users.Update(entity);
    }

    public void Delete(User entity)
    {
        _dbContext.Users.Remove(entity);
    }
}
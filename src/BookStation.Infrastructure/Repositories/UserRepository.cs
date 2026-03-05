using BookStation.Core.Entities.UserAggregate;
using BookStation.Core.Repositories;
using BookStation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

    public void Update(User entity)
    {
        _dbContext.Users.Update(entity);
    }

    public void Delete(User entity)
    {
        _dbContext.Users.Remove(entity);
    }

    Task<User?> IUserRepository.GetByEmailAsync(string email, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    Task<bool> IUserRepository.ExistsByEmailAsync(string email, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    Task<User?> IUserRepository.GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    void IUserRepository.Update(object user)
    {
        throw new NotImplementedException();
    }

    Task IUserRepository.AddAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
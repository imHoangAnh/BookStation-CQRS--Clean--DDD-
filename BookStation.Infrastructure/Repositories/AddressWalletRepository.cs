using BookStation.Domain.Entities.UserAggregate;
using BookStation.Domain.Repositories;
using BookStation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookStation.Infrastructure.Repositories;

public class AddressWalletRepository : IAddressWalletRepository
{
    private readonly BookStationDbContext _context;

    public AddressWalletRepository(BookStationDbContext context)
    {
        _context = context;
    }

    public async Task<AddressWallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AddressWallets
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<AddressWallet>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AddressWallets
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<AddressWallet?> GetDefaultByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AddressWallets
            .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault, cancellationToken);
    }

    public async Task AddAsync(AddressWallet address, CancellationToken cancellationToken = default)
    {
        await _context.AddressWallets.AddAsync(address, cancellationToken);
    }

    public void Update(AddressWallet address)
    {
        _context.AddressWallets.Update(address);
    }

    public void Delete(AddressWallet address)
    {
        _context.AddressWallets.Remove(address);
    }
}



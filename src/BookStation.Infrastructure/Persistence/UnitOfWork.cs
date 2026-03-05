using Microsoft.EntityFrameworkCore.Storage;
using BookStation.Application.Abstractions;

namespace BookStation.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly BookStationDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    public UnitOfWork(BookStationDbContext context) { _context = context; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    //public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    //    => _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null) throw new InvalidOperationException("No active transaction.");
        await _context.SaveChangesAsync(cancellationToken);
        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction is null) return;
        await _currentTransaction.RollbackAsync();
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing) { _currentTransaction?.Dispose(); _context.Dispose(); }
        _disposed = true;
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

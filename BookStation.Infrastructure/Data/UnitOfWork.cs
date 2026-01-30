using BookStation.Core.SharedKernel;

namespace BookStation.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly BookStationDbContext _context;
    private bool _disposed;

    public UnitOfWork(BookStationDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }
}
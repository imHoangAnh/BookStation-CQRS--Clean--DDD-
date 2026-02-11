using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Core.SharedKernel;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Day tat cac cac thay doi dang cho tu bo nho xuong co so du lieu 
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    Task RollbackTransactionAsync();
}

/// <summary>
/// IDisposable: Provides a mechanism for releasing unmanaged resources.
/// La interface dung de giai phong tai nguyen khong duoc quan ly mot cach chu dong va dung thoi diem, thay vi dung GC(Garbage Collector).
/// </summary>

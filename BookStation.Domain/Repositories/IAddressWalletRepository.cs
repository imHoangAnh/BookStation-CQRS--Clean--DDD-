using BookStation.Domain.Entities.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Domain.Repositories;

public interface IAddressWalletRepository
{
    //Get an AddressWallet by Id
    Task<AddressWallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    //Get all AddressWallets by UserId
    Task<List<AddressWallet>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    //Get the default AddressWallet by UserId
    Task<AddressWallet?> GetDefaultByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    //Add a new AddressWallet
    Task AddAsync(AddressWallet address, CancellationToken cancellationToken = default);
    //Update an existing AddressWallet
    void Update(AddressWallet address);
    //Delete an AddressWallet
    void Delete(AddressWallet address);
}


/* CancellationToken là một cơ chế cho phép hủy bỏ (cancel) 
 * các thao tác bất đồng bộ (asynchronous operations) đang chạy.
 * Tac dung: huy bo thao tac khi khong can thiet, vi du: khi usser thuc hien mot request ton thoi gian (nhu query db)
 * nhung sau do ho lai dong trinh duyet hoac chuyen sang trang khac, luc nay ta co the su dung CancellationToken de:
 * Tiet kiem tai nguyen
 * Giai phong ket noi db
 * Tranh xu ly du lieu khong can thiet
 * 
 * Cach su dung:
 * public async Task<AddressWallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
 *{
 *   return await _context.AddressWallets
 *       .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
 *   // Entity Framework sẽ check CancellationToken và dừng query nếu token bị cancelled
 *} 
 * --> code se chay nhu sau
 * // Tạo CancellationTokenSource
 *   var cts = new CancellationTokenSource();   
 *
 *  // Gọi method với token
 *   var task = GetByIdAsync(userId, cts.Token);
 *
 *   // Nếu cần hủy (ví dụ: user click Cancel button)
 *   cts.Cancel();
 *
 * Task sẽ throw OperationCanceledException
 */
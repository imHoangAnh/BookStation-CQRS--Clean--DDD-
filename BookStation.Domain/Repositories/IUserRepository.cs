using System;
using System.Collections.Generic;
using System.Text;
using BookStation.Core.SharedKernel;
using BookStation.Domain.Entities.UserAggregate;

namespace BookStation.Domain.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    /// Gets user by email.
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellation = default);

    /// Checks if a user with the given email exists.
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellation = default);

    /// Gets user with their roles.
    //Task<User?> GetWithRolesAsync(Guid id, CancellationToken cancellation = default);

    /// Gets user with their seller profile.
    //Task<User?> GetWithSellerProfileAsync(Guid id, CancellationToken cancellation = default);
}

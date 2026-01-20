using System;
using System.Collections.Generic;
using System.Text;
using BookStation.Core.SharedKernel;
using BookStation.Domain.Entities.UserAggregate;
using Microsoft.EntityFrameworkCore;


namespace BookStation.Domain.Repositories;

public interface IRoleRepository : IRepository<Role, long>
{
}

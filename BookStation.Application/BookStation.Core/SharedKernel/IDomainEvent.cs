using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Core.SharedKernel;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}

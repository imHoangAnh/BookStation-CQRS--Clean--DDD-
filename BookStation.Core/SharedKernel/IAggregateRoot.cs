using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Core.SharedKernel;

public interface IAggregateRoot
{
    IReadOnlyList<DomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
/// <summary>
/// La mot nhom cac doi tuong du lieu duoc doi xu nhu mot the thong nhat trong he thong
/// vd: user aggregate phai co profile, address, order...
/// order aggregate phai co order items, payment info..
/// Giup repository chi can thao tac voi aggregate root ma khong can quan tam den cac doi tuong con ben trong no
/// </summary> 

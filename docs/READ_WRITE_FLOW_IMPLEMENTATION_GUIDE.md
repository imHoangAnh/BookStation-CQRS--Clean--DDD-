# Hướng dẫn implement: 1 database, 2 luồng đọc/ghi tách bạch

Tài liệu này mô tả cách áp dụng pattern **một database, hai luồng logic** (đọc riêng, ghi riêng) vào một dự án .NET khác. Mục tiêu: **một connection string**, nhưng code tách rõ **luồng đọc** (Query) và **luồng ghi** (Command).

---

## 1. Mục tiêu

| Yêu cầu | Cách đạt được |
|--------|----------------|
| Chỉ 1 database | Một DbContext, một connection string |
| Luồng đọc tách bạch | Query layer chỉ inject `IReadDbContext` (chỉ có DbSet, không có SaveChanges) |
| Luồng ghi tách bạch | Command/Application layer dùng `IUnitOfWork` + repositories, transaction wrap command |

**Lợi ích:** Code rõ ràng (đọc vs ghi), dễ test, sau này nếu cần tách read replica chỉ cần đổi implementation của `IReadDbContext`.

---

## 2. Cấu trúc project gợi ý

```
YourSolution/
├── YourSolution.Core/           # Shared kernel (IUnitOfWork, v.v.)
├── YourSolution.Domain/         # Entities, repositories interfaces
├── YourSolution.Application/    # Commands, CommandHandlers, Behaviors (Transaction)
├── YourSolution.Query/          # Queries, QueryHandlers, IReadDbContext
├── YourSolution.Infrastructure/ # DbContext, repositories, DI
└── YourSolution.Api/            # Controllers, Program.cs
```

Quan trọng: **Query** nên có project riêng (hoặc folder riêng) và reference **Abstractions** chứa `IReadDbContext`. **Infrastructure** reference **Query** để implement `IReadDbContext` (DbContext implement interface đó).

---

## 3. Các bước implement

### Bước 1: Tạo `IUnitOfWork` (Core / SharedKernel)

**Vị trí:** `YourSolution.Core/SharedKernel/IUnitOfWork.cs` (hoặc project tương đương).

```csharp
namespace YourSolution.Core.SharedKernel;

/// <summary>
/// Unit of Work: phối hợp ghi thay đổi xuống database (một transaction).
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

- Chỉ dùng cho **luồng ghi**: Command handlers và repositories không gọi trực tiếp `DbContext.SaveChangesAsync`, mà gọi qua `IUnitOfWork.SaveChangesAsync`.

---

### Bước 2: Tạo `IReadDbContext` (Query / Abstractions)

**Vị trí:** `YourSolution.Query/Abstractions/IReadDbContext.cs`.

- Interface **chỉ đọc**: chỉ khai báo các `DbSet<T>` cần cho query, **không** có `SaveChanges` hay `Add`/`Update`/`Remove`.
- Project Query cần reference: **Microsoft.EntityFrameworkCore** (để dùng `DbSet<>`), **Domain** (entities).

```csharp
using Microsoft.EntityFrameworkCore;
// using YourSolution.Domain.Entities.* — thay đúng namespace entity của bạn

namespace YourSolution.Query.Abstractions;

/// <summary>
/// Read-only context cho luồng đọc. Chỉ expose DbSet để query.
/// </summary>
public interface IReadDbContext
{
    DbSet<YourEntity> YourEntities { get; }
    // DbSet<OtherEntity> OtherEntities { get; }
    // ... thêm các DbSet tương ứng entity bạn cần đọc
}
```

**Lưu ý:** Chỉ khai báo những `DbSet` mà Query handlers thực sự dùng. Càng gọn càng dễ bảo trì.

---

### Bước 3: DbContext implement cả `IUnitOfWork` và `IReadDbContext`

**Vị trí:** `YourSolution.Infrastructure/Persistence/YourDbContext.cs`.

- Kế thừa `DbContext`, implement **cả hai** `IUnitOfWork` và `IReadDbContext`.
- **Một** class, **một** connection string; đăng ký một lần trong DI, map ra hai interface.

```csharp
using Microsoft.EntityFrameworkCore;
using YourSolution.Core.SharedKernel;
using YourSolution.Query.Abstractions;
// using YourSolution.Domain.Entities.* 

namespace YourSolution.Infrastructure.Persistence;

public class YourDbContext : DbContext, IUnitOfWork, IReadDbContext
{
    public YourDbContext(DbContextOptions<YourDbContext> options)
        : base(options) { }

    // DbSet — có thể dùng Set<T>() cho cả IReadDbContext và nội bộ DbContext
    public DbSet<YourEntity> YourEntities => Set<YourEntity>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Tùy chọn: dispatch domain events, audit, v.v.
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

- **Infrastructure** cần reference: **Core** (IUnitOfWork), **Query** (IReadDbContext), **Domain**, **EF Core**.

---

### Bước 4: Đăng ký DI — một DbContext, hai interface

**Vị trí:** `YourSolution.Infrastructure/DependencyInjection.cs` (hoặc nơi bạn cấu hình Infrastructure).

```csharp
services.AddDbContext<YourDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(YourDbContext).Assembly.FullName)));

// Cùng một instance: vừa là IUnitOfWork (ghi), vừa là IReadDbContext (đọc)
services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<YourDbContext>());
services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<YourDbContext>());
```

Kết quả: **một connection string**, **một** DbContext; luồng đọc và luồng ghi dùng chung DB nhưng tách bạch qua interface.

---

### Bước 5: Luồng đọc — Query handlers chỉ dùng `IReadDbContext`

- Mọi **query** (get by id, list, search, v.v.) nên nằm trong **Query layer** (MediatR `IRequestHandler` hoặc tương đương).
- Handler **chỉ** inject `IReadDbContext`, **không** inject DbContext hay IUnitOfWork.

```csharp
// Ví dụ: YourSolution.Query/Handlers/GetItemByIdQueryHandler.cs
public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto?>
{
    private readonly IReadDbContext _db;

    public GetItemByIdQueryHandler(IReadDbContext db) => _db = db;

    public async Task<ItemDto?> Handle(GetItemByIdQuery request, CancellationToken ct)
    {
        return await _db.YourEntities
            .AsNoTracking()
            .Where(e => e.Id == request.Id)
            .Select(e => new ItemDto { ... })
            .FirstOrDefaultAsync(ct);
    }
}
```

- Nên dùng `.AsNoTracking()` cho query chỉ đọc để rõ ràng và tránh track không cần thiết.

---

### Bước 6: Luồng ghi — Repositories + IUnitOfWork, Command handlers không gọi SaveChanges

- **Repositories** nhận **DbContext** (hoặc IUnitOfWork nếu bạn expose DbSet qua IUnitOfWork — thường repositories nhận DbContext để truy cập DbSet). Thực tế phổ biến: repository nhận `YourDbContext` (hoặc interface có DbSet), còn **commit** thì do Application layer gọi `IUnitOfWork.SaveChangesAsync()`.
- **Command handlers** inject repository + `IUnitOfWork`. Sau khi gọi repository (Add/Update/Delete), handler **không** gọi `SaveChangesAsync` — giao cho pipeline behavior.

Ví dụ repository (constructor nhận DbContext):

```csharp
// Infrastructure/Repositories/YourRepository.cs
public class YourRepository : IYourRepository
{
    private readonly YourDbContext _context;
    public YourRepository(YourDbContext context) => _context = context;

    public IUnitOfWork UnitOfWork => _context;

    public async Task<YourEntity?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.YourEntities.FirstOrDefaultAsync(e => e.Id == id, ct);

    public void Add(YourEntity entity) => _context.YourEntities.Add(entity);
}
```

Command handler chỉ gọi repository, không gọi SaveChanges:

```csharp
// Application/Commands/CreateItemCommandHandler.cs
public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Guid>
{
    private readonly IYourRepository _repo;

    public CreateItemCommandHandler(IYourRepository repo) => _repo = repo;

    public async Task<Guid> Handle(CreateItemCommand request, CancellationToken ct)
    {
        var entity = new YourEntity { ... };
        _repo.Add(entity);
        // Không gọi SaveChanges ở đây — để TransactionBehavior gọi IUnitOfWork.SaveChangesAsync
        return entity.Id;
    }
}
```

---

### Bước 7: Transaction behavior — gọi `IUnitOfWork.SaveChangesAsync` sau command

Để mọi command chạy trong một transaction và commit một chỗ, dùng MediatR pipeline behavior:

```csharp
// Application/Behaviors/TransactionBehavior.cs
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Bỏ qua query — chỉ command mới cần SaveChanges
        if (typeof(TRequest).Name.EndsWith("Query", StringComparison.OrdinalIgnoreCase))
            return await next();

        var response = await next();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return response;
    }
}
```

Đăng ký behavior (trong Application DI):

```csharp
// Application/DependencyInjection.cs
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
```

Như vậy luồng ghi luôn đi qua `IUnitOfWork`, một nơi commit.

---

## 4. Checklist áp dụng sang dự án khác

| # | Việc cần làm |
|---|----------------|
| 1 | Tạo `IUnitOfWork` (SaveChangesAsync) trong Core/SharedKernel |
| 2 | Tạo `IReadDbContext` (chỉ DbSet) trong Query/Abstractions |
| 3 | DbContext implement cả `IUnitOfWork` và `IReadDbContext` |
| 4 | DI: một DbContext, đăng ký thêm `IUnitOfWork` và `IReadDbContext` cùng instance |
| 5 | Query handlers: chỉ inject `IReadDbContext`, dùng AsNoTracking khi có thể |
| 6 | Repositories/Command handlers: dùng DbContext (hoặc repo) cho ghi, không gọi SaveChanges trong handler |
| 7 | Transaction behavior: với request không phải Query, gọi `IUnitOfWork.SaveChangesAsync()` sau next() |

---

## 5. Tóm tắt luồng

- **Đọc:** API → MediatR Query → Query handler → `IReadDbContext` → EF Core → DB (cùng connection).
- **Ghi:** API → MediatR Command → Command handler → Repository (DbContext) → Behavior gọi `IUnitOfWork.SaveChangesAsync()` → DB (cùng connection).

Một database, một connection string; hai luồng tách bạch nhờ hai abstraction `IReadDbContext` (đọc) và `IUnitOfWork` (ghi).

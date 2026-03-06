using BookStation.Application.Abstractions;
using BookStation.Application.Contracts;
using BookStation.Application.Services;
using BookStation.Core.Repositories;
using BookStation.Infrastructure.Data;
using BookStation.Infrastructure.Repositories;
using BookStation.Infrastructure.Services;
using BookStation.Query.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<BookStationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register BookStationDbContext as DbContext (for generic usage)
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<BookStationDbContext>());

        // Read flow: one DbContext instance exposed as IReadDbContext (Query layer)
        services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<BookStationDbContext>());

        // Unit of Work (write flow)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAddressWalletRepository, AddressWalletRepository>();
        services.AddScoped<IUserAddressRepository, AddressWalletRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IVoucherRepository, VoucherRepository>();

        // Services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddScoped<IPaymentService, DummyPaymentService>();

        return services;
    }
}
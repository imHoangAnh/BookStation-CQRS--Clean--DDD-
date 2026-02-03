using BookStation.Application.Contracts;
using BookStation.Core.SharedKernel;
using BookStation.Domain.Repositories;
using BookStation.Infrastructure.Data;
using BookStation.Infrastructure.Repositories;
using BookStation.Infrastructure.Services;
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

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();


        return services;
    }
}
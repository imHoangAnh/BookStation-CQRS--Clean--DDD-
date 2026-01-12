using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BookStation.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var currentAssembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(currentAssembly));
            
            services.AddValidatorsFromAssembly(currentAssembly);
            // Register application services here
            return services;
        }
    }   
}

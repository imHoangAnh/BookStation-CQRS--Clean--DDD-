using Microsoft.Extensions.DependencyInjection;

namespace BookStation.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Register application services here
            var currentAssembly = typeof(DependencyInjection).Assembly;
            return services;
        }
    }
}
    
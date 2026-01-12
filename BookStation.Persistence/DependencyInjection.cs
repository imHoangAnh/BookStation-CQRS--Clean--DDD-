using Microsoft.Extensions.DependencyInjection;

namespace BookStation.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            // Register application services here
            var currentAssembly = typeof(DependencyInjection).Assembly;
            return services;
        }
    }
}
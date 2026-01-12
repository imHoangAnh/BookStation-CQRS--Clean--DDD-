using Microsoft.Extensions.DependencyInjection;

namespace BookStation.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            // Register application services here
            var currentAssembly = typeof(DependencyInjection).Assembly;
            return services;
        }
    }
}
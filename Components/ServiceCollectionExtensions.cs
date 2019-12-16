using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Components.States;
using Microsoft.Extensions.DependencyInjection;

namespace Components
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationComponents(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<AppState>()
                .AddClasses(c=> c
                    .AssignableTo<TemporaryState>())
                .AsSelf()
                .WithScopedLifetime()
            );
            services.AddStorage();
        }
    }
}

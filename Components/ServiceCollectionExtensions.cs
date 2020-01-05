using System;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Components.Resources;
using Components.States;
using Core.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Components
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationComponents<TResourceManagerFactory>(this IServiceCollection services) where TResourceManagerFactory : class, IResourceManagerFactory
        {
            services.Scan(scan => scan
                .FromAssemblyOf<AppState>()
                .AddClasses(c => c
                    .AssignableTo<TemporaryState>())
                .AsSelf()
                .WithScopedLifetime()
            );
            services.AddStorage();

            //Register all resources
            services.AddCoreResources<TResourceManagerFactory>()
                .Register<LayoutResource>();
        }
    }
}

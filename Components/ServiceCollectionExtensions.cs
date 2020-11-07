using Blazored.LocalStorage;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Components.Resources;
using Core.Localization;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace Components
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationComponents<TResourceManagerFactory>(this IServiceCollection services) where TResourceManagerFactory : class, IResourceManagerFactory
        {
            services.AddStorage();


            //Register all resources
            services.AddCoreResources<TResourceManagerFactory>()
                .Register<LayoutResource>();

            services.AddFluxor(o =>
            {
                o.ScanAssemblies(typeof(Application).Assembly);
                o.UseReduxDevTools();
            });
            services.AddBlazoredLocalStorage();
        }
    }
}
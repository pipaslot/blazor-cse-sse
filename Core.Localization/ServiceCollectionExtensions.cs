using Microsoft.Extensions.DependencyInjection;

namespace Core.Localization
{
    public static class ServiceCollectionExtensions
    {
        public static ResourceDefinitions AddCoreResources<TResourceManagerFactory>(this IServiceCollection services) where TResourceManagerFactory : class, IResourceManagerFactory
        {
            var rc = new ResourceDefinitions();
            services.AddSingleton<ResourceDefinitions>(rc);
            services.AddSingleton<ResourceCollection>();
            services.AddSingleton<IResourceManagerFactory, TResourceManagerFactory>();
            return rc;
        }
    }
}

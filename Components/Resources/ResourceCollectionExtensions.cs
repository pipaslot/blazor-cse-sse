using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Components.Resources
{
    public static class ServiceCollectionExtensions
    {
        public static ResourceDefinitions AddResources(this IServiceCollection services)
        {
            var rc = new ResourceDefinitions();
            services.AddSingleton<ResourceDefinitions>(rc);
            services.AddSingleton<ResourceCollection>();
            return rc;
        }
    }
}

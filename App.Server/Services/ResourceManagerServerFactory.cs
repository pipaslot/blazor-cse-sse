using System;
using Core.Localization;

namespace App.Server.Services
{
    public class ResourceManagerServerFactory : IResourceManagerFactory
    {
        public ResourceManagerWithCulture Create(Type resourceType)
        {
            return new ResourceManagerServer(resourceType);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Components.Resources;
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

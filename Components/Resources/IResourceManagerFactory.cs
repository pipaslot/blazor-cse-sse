using System;
using System.Resources;

namespace Components.Resources
{
    public interface IResourceManagerFactory
    {
        ResourceManagerWithCulture Create(Type resourceType);
    }
}
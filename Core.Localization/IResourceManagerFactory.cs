using System;

namespace Core.Localization
{
    public interface IResourceManagerFactory
    {
        ResourceManagerWithCulture Create(Type resourceType);
    }
}
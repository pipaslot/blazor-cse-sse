using System;
using System.Net.Http;
using Core.Localization;

namespace App.Client.ApiServices
{
    public class ResourceManagerClientFactory : IResourceManagerFactory
    {
        private readonly HttpClient _httpClient;

        public ResourceManagerClientFactory(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public ResourceManagerWithCulture Create(Type resourceType)
        {
            return new ResourceManagerClient(_httpClient, resourceType);
        }
    }
}

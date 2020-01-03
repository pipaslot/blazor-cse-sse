using System;
using System.Net.Http;
using System.Reflection;
using Components.Resources;
using Components.States;

namespace App.Client.Services
{
    public class ResourceManagerFactory
    {
        private readonly HttpClient _httpClient;
        private readonly AppState _appState;

        public ResourceManagerFactory(HttpClient httpClient, AppState appState)
        {
            _httpClient = httpClient;
            _appState = appState;
        }

        public void ReplaceResourceManager<TResource>()
        {
            var newRm = Create<TResource>();
            _appState.ResourceManagers.Add(newRm);
            var fieldToHack = typeof(TResource).GetField("resourceMan", BindingFlags.Static | BindingFlags.NonPublic);
            fieldToHack.SetValue(null, newRm);
        }

        public ApiResourceManager Create<TResource>()
        {
            var classType = typeof(TResource);
            return new ApiResourceManager(_httpClient, classType);
        }
    }
}

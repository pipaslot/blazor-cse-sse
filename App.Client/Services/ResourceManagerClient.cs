using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Components.Resources;
using Core.Localization;
using Microsoft.AspNetCore.Components;

namespace App.Client.Services
{
    public class ResourceManagerClient : ResourceManagerWithCulture
    {
        private readonly HttpClient _httpClient;
        private readonly Type _classType;

        public ResourceManagerClient(HttpClient httpClient, Type classType) : base(classType.FullName, classType.Assembly)
        {
            _httpClient = httpClient;
            _classType = classType;
        }

        public override string GetString(string name, CultureInfo culture)
        {
            return GetString(name);
        }

        public override string GetString(string name)
        {
            if (_translations.TryGetValue(name, out string value))
            {
                return value;
            }
            return base.GetString(name);
        }

        private Dictionary<string, string> _translations = new Dictionary<string, string>();
        
        protected override async Task OnCultureChanged(string culture)
        {
            var typeName = _classType.AssemblyQualifiedName;
            _translations = await _httpClient.GetJsonAsync<Dictionary<string, string>>($"/api/languages/{culture}/resources?typeName=" + typeName);
        }
    }
}

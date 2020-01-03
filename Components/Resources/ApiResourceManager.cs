using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Resources;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Components.Resources
{
    public class ApiResourceManager : ResourceManager
    {
        private readonly HttpClient _httpClient;
        private readonly Type _classType;

        public ApiResourceManager(HttpClient httpClient, Type classType) : base(classType.FullName, classType.Assembly)
        {
            _httpClient = httpClient;
            _classType = classType;
        }
        
        public override string GetString(string name)
        {
            if (_translations.TryGetValue(name, out string value))
            {
                return value;
            }
            return base.GetString(name);
        }

        public override string GetString(string name, CultureInfo culture)
        {
            if (_translations.TryGetValue(name, out string value))
            {
                return value;
            }
            return base.GetString(name, culture);
        }

        private Dictionary<string, string> _translations = new Dictionary<string, string>();
        private Task<Dictionary<string, string>> _translationsTask;
        private string _currentCulture;
        internal async Task<Dictionary<string, string>> LoadTranslations(string culture)
        {
            if (_currentCulture != culture)
            {
                _translations.Clear();
                _translationsTask = null;
                _currentCulture = culture;
            }
            if (_translations.Count == 0)
            {
                if (_translationsTask == null)
                {
                    var typeName = _classType.AssemblyQualifiedName;
                    _translationsTask = _httpClient.GetJsonAsync<Dictionary<string, string>>(
                        $"/api/languages/{culture}/resources?typeName="+typeName);
                }

                _translations = await _translationsTask;
            }

            return _translations;

        }
    }
}

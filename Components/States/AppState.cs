using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Components.Resources;

namespace Components.States
{
    public class AppState : PersistedState<AppStateData>
    {
        private readonly ResourceCollection _resourceCollection;
        public AppState(LocalStorage localStorage, ResourceCollection resourceCollection) : base(localStorage)
        {
            _resourceCollection = resourceCollection;
        }
        
        public string Language => Data.Language;

        public async Task SetLanguage(string language)
        {
            Data.Language = language;
            await _resourceCollection.SetCulture(language);
            SaveChanges();
        }
        
        protected override async Task OnLoad()
        {
            await base.OnLoad();
            await _resourceCollection.SetCulture(Language);
        }
    }

    public class AppStateData
    {
        public string Language { get; set; } = "DE";
    }
}

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
        public List<ApiResourceManager> ResourceManagers { get; set; } = new List<ApiResourceManager>();
        public AppState(LocalStorage localStorage) : base(localStorage)
        {
        }
        
        public string Language => Data.Language;

        public async Task SetLanguage(string language)
        {
            Data.Language = language;
            await SwitchLanguage(language);
            SaveChanges();
        }

        private async Task SwitchLanguage(string language)
        {
            var tasks = ResourceManagers.Select(rm => rm.LoadTranslations(Language));
            await Task.WhenAll(tasks);

        }

        protected override async Task OnLoad()
        {
            await base.OnLoad();
            await SwitchLanguage(Language);
        }
    }

    public class AppStateData
    {
        public string Language { get; set; } = "DE";
    }
}

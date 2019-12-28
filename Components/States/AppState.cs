using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;

namespace Components.States
{
    public class AppState : PersistedState<AppStateData>
    {
        public AppState(LocalStorage localStorage) : base(localStorage)
        {
        }
        
        public string Language
        {
            get => Data.Language;
            set { Data.Language = value;
                SwitchLanguage(value);
                SaveChanges();
            }
        }

        private void SwitchLanguage(string language)
        {
            var culture = new CultureInfo(language);
            //CultureInfo.CurrentCulture = culture;
            Resources.Layout.Culture = culture;
        }

        protected override async Task OnLoad()
        {
            await base.OnLoad();
            SwitchLanguage(Language);
        }
    }
    public class AppStateData
    {
        public string Language { get; set; } = "DE";
    }
}

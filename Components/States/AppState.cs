using System;
using System.Collections.Generic;
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
                SaveChanges();
            }
        }
    }
    public class AppStateData
    {
        public string Language { get; set; } = "DE";
    }
}

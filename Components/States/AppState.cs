using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Components.StateAbstraction;

namespace Components.States
{
    public class AppState : State
    {
        private readonly LocalStorage _localStorage;

        public AppState(LocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        private string _language = "DE";

        public string Language
        {
            get => _language;
            set { _language = value;
                _localStorage.SetItemAsync("language",value);
                StateHasChanged();
            }
        }

       protected override async Task OnLoad()
       {
           var language = await _localStorage.GetItemAsync("language");
           if (!string.IsNullOrWhiteSpace(language))
           {
               _language = language;
           }
       }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using GeneralComponents.StateAbstraction;

namespace GeneralComponents.States
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
            get => //_localStorage.GetItemAsync("language").GetAwaiter().GetResult();//_language;
                _language;
            set { _language = value;
                _localStorage.SetItemAsync("language",value);
                StateHasChanged();
            }
        }

        //protected override async Task OnLoad()
        //{
        //    _language = await _localStorage.GetItemAsync<string>("language");
        //    if (string.IsNullOrWhiteSpace(_language))
        //    {
        //        _language = "EN";
        //    }
        //}
    }
}

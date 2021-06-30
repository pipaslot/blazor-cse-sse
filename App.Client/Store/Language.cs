using System.Threading.Tasks;
using Blazored.LocalStorage;
using Core.Localization;
using Fluxor;

namespace App.Client.Store
{
    public static class Language
    {
        public const string LocalStorageKey = nameof(Language);
        public const string DefaultLanguage = "en";
        public class State
        {
            public State(string language, bool loading)
            {
                Language = language;
                Loading = loading;
            }

            public string Language { get; }
            
            public bool Loading { get; }
        }
        public class PersistedState
        {
            public string Language { get; set; } = DefaultLanguage;
        }
        
        // ReSharper disable once UnusedType.Global
        public class Feature : Feature<State>
        {
            public override string GetName()
            {
                return nameof(Language);
            }

            protected override State GetInitialState()
            {
                return new State(DefaultLanguage, true);
            }
            public static async Task LoadPersistedStateAsync(ILocalStorageService localStorage, IDispatcher dispatcher)
            {
                var state = await localStorage.GetItemAsync<PersistedState>(Authentication.LocalStorageKey);
                var storedLanguage = state?.Language;
                var language = !string.IsNullOrWhiteSpace(storedLanguage) ? storedLanguage: DefaultLanguage;
                dispatcher.Dispatch(new ChangeLanguageAction(language));
            }
        }

        public class ChangeLanguageAction
        {
            public ChangeLanguageAction(string language)
            {
                Language = language;
            }

            public string Language { get;}
        }
        
        // ReSharper disable once UnusedMember.Global
        [ReducerMethod]
        public static State ReduceChangeLanguageAction(State state, ChangeLanguageAction action) => new State(state.Language, true);
        
        // ReSharper disable once UnusedMember.Global
        public class ChangeLanguageActionEffect : Effect<ChangeLanguageAction>
        {
            private readonly ILocalStorageService _localStorageService;
            private readonly ResourceCollection _resourceCollection;

            public ChangeLanguageActionEffect(ILocalStorageService localStorageService, ResourceCollection resourceCollection)
            {
                _localStorageService = localStorageService;
                _resourceCollection = resourceCollection;
            }

            protected override async Task HandleAsync(ChangeLanguageAction action, IDispatcher dispatcher)
            {
                await _resourceCollection.SetCulture(action.Language);
                await _localStorageService.SetItemAsync(LocalStorageKey, new PersistedState
                {
                    Language = action.Language
                });
                dispatcher.Dispatch(new StoreNewLanguageAction(action.Language));
            }
        }
        
        
        public class StoreNewLanguageAction
        {
            public StoreNewLanguageAction(string language)
            {
                Language = language;
            }

            public string Language { get;}
        }
        
        // ReSharper disable once UnusedMember.Global
        [ReducerMethod]
        public static State ReduceChangeLanguageAction(State state, StoreNewLanguageAction action) => new State(action.Language, false);
        

    }
}
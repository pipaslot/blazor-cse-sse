using System.Threading.Tasks;
using Blazored.LocalStorage;
using Fluxor;

namespace Components.Store
{
    public static class Language
    {
        public const string LocalStorageKey = nameof(Authentication);
        public const string DefaultLanguage = "de";
        public class State
        {
            public State(string language)
            {
                Language = language;
            }

            public string Language { get; }
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
                return new State(DefaultLanguage);
            }
            public static async Task LoadPersistedStateAsync(ILocalStorageService localStorage, IDispatcher dispatcher)
            {
                var state = await localStorage.GetItemAsync<PersistedState>(Authentication.LocalStorageKey);
                if (state != null){
                    dispatcher.Dispatch(new ChangeLanguageAction(state.Language));
                }
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
        public static State ReduceChangeLanguageAction(State state, ChangeLanguageAction action) => new State(action.Language);
        
        // ReSharper disable once UnusedMember.Global
        public class ChangeLanguageActionEffect : Effect<ChangeLanguageAction>
        {
            private readonly ILocalStorageService _localStorageService;

            public ChangeLanguageActionEffect(ILocalStorageService localStorageService)
            {
                _localStorageService = localStorageService;
            }

            protected override async Task HandleAsync(ChangeLanguageAction action, IDispatcher dispatcher)
            {
                await _localStorageService.SetItemAsync(LocalStorageKey, new PersistedState
                {
                    Language = action.Language
                });
            }
        }

    }
}
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Fluxor;

namespace Components.Store
{
    public static class Init
    {
        public class LoadPersistedStateAction
        {
            
        }
        // ReSharper disable once UnusedType.Global
        public class LoadPersistedStateEffect : Effect<LoadPersistedStateAction>
        {
            private readonly ILocalStorageService _localStorage;

            public LoadPersistedStateEffect(ILocalStorageService localStorage)
            {
                _localStorage = localStorage;
            }

            protected override async Task HandleAsync(LoadPersistedStateAction action, IDispatcher dispatcher)
            {
                var tasks = new []
                {
                    Authentication.Feature.LoadPersistedStateAsync(_localStorage, dispatcher),
                    Language.Feature.LoadPersistedStateAsync(_localStorage, dispatcher)
                };
                await Task.WhenAll(tasks);
            }
        }
    }
}
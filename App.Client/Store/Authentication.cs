using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Core.Jwt;
using Fluxor;

namespace App.Client.Store
{
    public static class Authentication
    {
        public const string LocalStorageKey = nameof(Authentication);
        public class State
        {
            public State(JwtToken? bearerToken, string userName)
            {
                BearerToken = bearerToken;
                UserName = userName;
            }

            public JwtToken? BearerToken { get; }

            public string UserName { get; }

            public bool IsAuthenticated => BearerToken != null && BearerToken.ValidTo > DateTime.UtcNow;
        }

        public class PersistedState
        {
            public JwtToken? BearerToken { get; set; }

            public string? UserName { get; set; }
        }

        // ReSharper disable once UnusedType.Global
        public class Feature : Feature<State>
        {
            public override string GetName()
            {
                return nameof(Authentication);
            }

            protected override State GetInitialState()
            {
                return new State(null, "");
            }
            public static async Task LoadPersistedStateAsync(ILocalStorageService localStorage, IDispatcher dispatcher)
            {
                var auth = await localStorage.GetItemAsync<PersistedState>(LocalStorageKey);
                if (auth?.BearerToken != null){
                    dispatcher.Dispatch(new SignInAction(auth.BearerToken, auth.UserName ?? ""));
                }
            }
        }
        
        #region Sing In

        public class SignInAction
        {
            public SignInAction(JwtToken bearerToken, string userName)
            {
                BearerToken = bearerToken;
                UserName = userName;
            }

            public JwtToken BearerToken { get; }
            public string UserName { get; }
        }

        [ReducerMethod]
        // ReSharper disable once UnusedMember.Global
        public static State ReduceSignInAction(State state, SignInAction action) => new State(action.BearerToken, action.UserName);
        
        // ReSharper disable once UnusedMember.Global
        public class SignInActionEffect : Effect<SignInAction>
        {
            private readonly ILocalStorageService _localStorageService;

            public SignInActionEffect(ILocalStorageService localStorageService)
            {
                _localStorageService = localStorageService;
            }

            protected override async Task HandleAsync(SignInAction action, IDispatcher dispatcher)
            {
                await _localStorageService.SetItemAsync(LocalStorageKey, new PersistedState
                {
                    BearerToken = action.BearerToken,
                    UserName = action.UserName
                });
            }
        }

        #endregion

        #region Sign out

        public class SignOutAction
        {
        }

        // ReSharper disable once UnusedMember.Global
        [ReducerMethod]
        public static State ReduceSignOutAction(State state, SignOutAction action) => new State(null, "");
        
        #endregion
    }
}
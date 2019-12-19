using System.Threading.Tasks;
using App.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace App.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJSRuntime _jsRuntime;

        public AuthService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        
        public async Task SignOut()
        {
            await _jsRuntime.InvokeVoidAsync("location.replace","/api/auth/sign-out");
        }

        public async Task SignIn(string username, string password)
        {
            //TODO Hash password
            await _jsRuntime.InvokeVoidAsync("location.replace","/api/auth/sign-in?username=" + username + "&passwordHash=" + password);
        }
    }
}

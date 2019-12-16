using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Auth
{
    public class AuthService : IAuthService
    {
        public Task<string[]> GetUserPermissions()
        {
            return Task.FromResult(new string[]
            {
                "AAAA",
                "BBBB"
            });
        }
    }
}

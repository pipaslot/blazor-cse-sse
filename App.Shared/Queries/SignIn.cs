using Core.Jwt;
using App.Shared.CQRSAbstraction;

namespace App.Shared.Queries
{
    public static class SignIn
    {
        public class Query : IQuery<Result>
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class Result
        {
            public bool Success { get; set; }
            public JwtToken AccessToken { get; set; }
            public string Username { get; set; }
        }
    }
}

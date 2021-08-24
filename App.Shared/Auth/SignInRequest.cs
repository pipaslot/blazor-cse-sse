using Core.Jwt;
using Pipaslot.Mediator.Abstractions;

namespace App.Shared.Auth
{
    public static class SignInRequest
    {
        public class Query : IRequest<Result>
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

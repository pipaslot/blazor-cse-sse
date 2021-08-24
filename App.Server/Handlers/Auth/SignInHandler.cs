using Core.Jwt;
using Pipaslot.Mediator.Abstractions;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Auth;

namespace App.Server.Handlers.Auth
{
    public class SignInHandler : IRequestHandler<SignInRequest.Query, SignInRequest.Result>
    {
        private readonly IOptions<AuthOptions> _authOptions;

        public SignInHandler(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions;
        }

        public Task<SignInRequest.Result> Handle(SignInRequest.Query request, CancellationToken cancellationToken)
        {
            var options = _authOptions.Value;
            var id = 5;
            var token = new JwtTokenBuilder()
                .AddSecurityKey(options.SecretKey)
                .AddSubject(request.Username)
                .AddIssuer(options.Issuer)
                .AddAudience(options.Audience)
                .AddClaim("MembershipId", id.ToString())
                //.AddClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role.ToString())
                .AddExpiry(options.TokenExpirationInMinutes)
                .Build();
            return Task.FromResult(new SignInRequest.Result
            {
                Success = true,
                AccessToken = token,
                Username = request.Username
            });
        }
    }
}

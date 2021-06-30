using App.Shared.Queries;
using Core.Jwt;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace App.Server.QueryHandlers
{
    public class SignInHandler : IQueryHandler<SignIn.Query, SignIn.Result>
    {
        private readonly IOptions<AuthOptions> _authOptions;

        public SignInHandler(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions;
        }

        public Task<SignIn.Result> Handle(SignIn.Query request, CancellationToken cancellationToken)
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
            return Task.FromResult(new SignIn.Result
            {
                Success = true,
                AccessToken = token,
                Username = request.Username
            });
        }
    }
}

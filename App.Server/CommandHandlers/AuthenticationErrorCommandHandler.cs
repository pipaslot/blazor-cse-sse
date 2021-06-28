using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Commands;
using Core.Mediator.Abstractions;

namespace App.Server.CommandHandlers
{
    // ReSharper disable once UnusedMember.Global
    public class AuthenticationErrorCommandHandler : CommandHandler<AuthenticationError.Command>
    {
        protected override Task Execute(AuthenticationError.Command command, CancellationToken cancellationToken)
        {
            //TODO Authentication on command level
            throw new AuthenticationException("Authentication failed");
        }
    }
}

using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.DemoActions;
using Pipaslot.Mediator;

namespace App.Server.Handlers.DemoActions
{
    // ReSharper disable once UnusedMember.Global
    public class AuthenticationErrorMessageHandler : IMessageHandler<AuthenticationErrorMessage.Command>
    {
        public Task Handle(AuthenticationErrorMessage.Command command, CancellationToken cancellationToken)
        {
            //TODO Authentication on command level
            throw new AuthenticationException("Authentication failed");
        }
    }
}

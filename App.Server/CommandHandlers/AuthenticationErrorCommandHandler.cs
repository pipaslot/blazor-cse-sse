using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Commands;
using Core.Mediator.CQRSExtensions;

namespace App.Server.CommandHandlers
{
    // ReSharper disable once UnusedMember.Global
    public class AuthenticationErrorCommandHandler : ICommandHandler<AuthenticationError.Command>
    {
        public Task<object> Handle(AuthenticationError.Command command, CancellationToken cancellationToken)
        {
            //TODO Authentication on command level
            throw new AuthenticationException();
        }
    }
}

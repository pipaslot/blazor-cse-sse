using System.Threading;
using System.Threading.Tasks;

namespace App.Shared.Mediator
{
    
    /// <summary>Defines a handler for a command</summary>
    /// <typeparam name="TCommand">The type of command being handled</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>Handles a notification</summary>
        /// <param name="command">The command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task Handle(TCommand command, CancellationToken cancellationToken);
    }
}
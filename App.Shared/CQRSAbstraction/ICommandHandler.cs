using Core.Mediator.Abstractions;

namespace App.Shared.CQRSAbstraction
{
    /// <summary>Defines a handler for a command</summary>
    /// <typeparam name="TCommand">The type of command being handled</typeparam>
    public interface ICommandHandler<in TCommand> : IMessageHandler<TCommand>
        where TCommand : ICommand
    {
    }
}
// ReSharper disable once CheckNamespace
namespace Core.Mediator.Abstractions
{
    
    /// <summary>Defines a handler for a command</summary>
    /// <typeparam name="TCommand">The type of command being handled</typeparam>
    public interface ICommandHandler<in TCommand> : IHandler<TCommand, object?>
        where TCommand : ICommand
    {
    }
}
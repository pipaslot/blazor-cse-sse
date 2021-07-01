using Pipaslot.Mediator.Abstractions;

namespace App.Shared.CQRSAbstraction
{
    /// <summary>
    /// Command action with specific pipeline middlewares and handlers.
    /// </summary>
    public interface ICommand : IMessage
    {

    }
}
using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Abstractions
{
    /// <summary>Defines a default request handler</summary>
    /// <typeparam name="TEvent">The type of event being handled</typeparam>
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        /// <summary>Handles an event</summary>
        /// <param name="event">The event</param>
        Task Handle(TEvent @event, CancellationToken cancellationToken);
    }
}

using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Pipeline behavior to surround the handler.
    /// Implementations add additional behavior and await the next delegate.
    /// </summary>
    public interface IEventMiddleware
    {
        /// <summary>
        /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
        /// </summary>
        /// <param name="event">Incoming event</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <returns>Awaitable task</returns>
        Task Handle<TEvent>(TEvent @event, CancellationToken cancellationToken, MiddlewareDelegate next) where TEvent : IEvent;
    }
}

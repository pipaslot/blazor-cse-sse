using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Pipeline behavior to surround the handler.
    /// Implementations add additional behavior and await the next delegate.
    /// </summary>
    public interface IEventPipeline
    {
        /// <summary>
        /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
        /// </summary>
        /// <param name="event">Incoming event</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <returns>Awaitable task</returns>
        Task Handle<TEvent>(TEvent @event, CancellationToken cancellationToken, EventHandlerDelegate next) where TEvent : IEvent;
    }

    /// <summary>
    /// Represents an async continuation for the next task to execute in the pipeline
    /// </summary>
    /// <returns>Awaitable task</returns>
    public delegate Task EventHandlerDelegate();
}

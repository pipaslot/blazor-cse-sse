using System.Threading;
using System.Threading.Tasks;

namespace App.Shared.Mediator
{
    /// <summary>
    /// Pipeline behavior to surround the handler.
    /// Implementations add additional behavior and await the next delegate.
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    public interface ICommandPipeline<in TRequest> where TRequest : notnull
    {
        /// <summary>
        /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
        /// </summary>
        /// <param name="request">Incoming request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <returns>Awaitable task</returns>
        Task Handle(TRequest request, CancellationToken cancellationToken, QueryHandlerDelegate next);
    }

    /// <summary>
    /// Represents an async continuation for the next task to execute in the pipeline
    /// </summary>
    /// <returns>Awaitable task</returns>
    public delegate Task QueryHandlerDelegate();
}
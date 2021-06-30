using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Pipeline behavior to surround the handler.
    /// Implementations add additional behavior and await the next delegate.
    /// </summary>
    public interface IRequestMiddleware
    {
        /// <summary>
        /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
        /// </summary>
        /// <param name="request">Incoming request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <returns>Awaitable task returning the <typeparamref name="TResponse"/></returns>
        Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, MiddlewareDelegate<TResponse> next) where TRequest : IRequest<TResponse>;
    }
}
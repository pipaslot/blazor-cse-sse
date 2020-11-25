using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Pipeline behavior to surround the handler.
    /// Implementations add additional behavior and await the next delegate.
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public interface IPipeline<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Returns true if pipeline should be applied to target query
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool CanHandle(TRequest request);

        /// <summary>
        /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
        /// </summary>
        /// <param name="request">Incoming request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <returns>Awaitable task returning the <typeparamref name="TResponse"/></returns>
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next);
    }
    
    /// <summary>
    /// Represents an async continuation for the next task to execute in the pipeline
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <returns>Awaitable task returning a <typeparamref name="TResponse" /></returns>
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
}
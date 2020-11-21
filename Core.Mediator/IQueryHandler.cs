using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator
{
    
    /// <summary>Defines a handler for a query</summary>
    /// <typeparam name="TQuery">The type of query being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        /// <summary>Handles a request</summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from the request</returns>
        Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken);
    }
}
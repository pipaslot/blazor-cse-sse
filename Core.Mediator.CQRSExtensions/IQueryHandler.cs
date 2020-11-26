using Core.Mediator.Abstractions;

namespace Core.Mediator.CQRSExtensions
{
    
    /// <summary>Defines a handler for a query</summary>
    /// <typeparam name="TQuery">The type of query being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public interface IQueryHandler<in TQuery, TResponse> : IHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
    }
}
using Core.Mediator.Abstractions;

namespace App.Shared.CQRSAbstraction
{
    /// <summary>
    /// Query action with specific pipeline middlewares and handlers.
    /// </summary>
    /// <typeparam name="TResponse">Result data returned from query execution</typeparam>
    public interface IQuery<out TResponse> : IQuery, IRequest<TResponse>
    {
        
    }

    /// <summary>
    /// Marker interface for IQuery action type. 
    /// Use only for pipeline configuration to define middlewares applicable for this action type.
    /// </summary>
    public interface IQuery : IRequest
    {
        
    }
}
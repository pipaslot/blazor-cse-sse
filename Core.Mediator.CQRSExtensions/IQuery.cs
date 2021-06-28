// ReSharper disable once CheckNamespace
namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Query action with own specific pipelines and handlers.
    /// </summary>
    /// <typeparam name="TResponse">Result data returned from query execution</typeparam>
    public interface IQuery<out TResponse> : IQuery, IRequest<TResponse>
    {
        
    }
    /// <summary>
    /// Marker interface for IRequest only. DO NOT INHERIT!
    /// </summary>
    public interface IQuery : IRequest
    {
        
    }
}
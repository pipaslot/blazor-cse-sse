// ReSharper disable once CheckNamespace
namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Query marked 
    /// </summary>
    /// <typeparam name="TResponse">Result data returned from query execution</typeparam>
    public interface IQuery<out TResponse> : IQuery, IRequest<TResponse>
    {
        
    }
    /// <summary>
    /// Query marker 
    /// </summary>
    public interface IQuery : IRequest
    {
        
    }
}
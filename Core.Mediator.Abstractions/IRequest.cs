namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Base request marker. All derived types can have own specific pipelines and handlers
    /// </summary>
    /// <typeparam name="TResponse">Result data returned from query execution</typeparam>
    public interface IRequest<out TResponse> : IRequest
    {
        
    }
    /// <summary>
    /// Marker interface
    /// </summary>
    public interface IRequest
    {

    }
}
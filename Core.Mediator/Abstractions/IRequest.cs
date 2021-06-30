namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Action which returns data. All derived types can have own specific pipelines and handlers.
    /// </summary>
    /// <typeparam name="TResponse">Result data returned from handler execution</typeparam>
    public interface IRequest<out TResponse> : IRequest
    {
        
    }
    /// <summary>
    /// Marker interface for IRequest only. DO NOT INHERIT!
    /// </summary>
    public interface IRequest : IActionMarker
    {

    }
}
// ReSharper disable once CheckNamespace
namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Command marker
    /// </summary>
    public interface ICommand : IRequest<object?>
    {
        
    }
}
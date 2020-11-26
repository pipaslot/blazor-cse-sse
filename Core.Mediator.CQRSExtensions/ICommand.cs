using Core.Mediator.Abstractions;

namespace Core.Mediator.CQRSExtensions
{
    /// <summary>
    /// Command marker
    /// </summary>
    public interface ICommand : IRequest<object>
    {
        
    }
}
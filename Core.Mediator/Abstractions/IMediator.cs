using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Abstractions
{
    /// <summary>
    ///     Request / Event dispatched
    /// </summary>
    public interface IMediator
    {
        Task<MediatorResponse<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        Task<MediatorResponse> Fire(IEvent @event, CancellationToken cancellationToken = default);
    }
}

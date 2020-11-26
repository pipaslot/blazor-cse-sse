using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Abstractions
{
    /// <summary>
    ///     Command / Query dispatched
    /// </summary>
    public interface IMediator
    {
        Task<MediatorResponse<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}

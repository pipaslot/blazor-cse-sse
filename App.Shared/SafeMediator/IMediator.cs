using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace App.Shared.SafeMediator
{
    /// <summary>
    /// Replace MediatR mediator to provide exception handling
    /// </summary>
    public interface IMediator
    {
        Task<MediatorResponse<TResponse>> Send<TResponse>(IRequest<TResponse> request,
            CancellationToken cancellationToken = default);

        Task<MediatorResponse> Publish<TNotification>(TNotification request, CancellationToken cancellationToken = default)
            where TNotification : INotification;
    }
}

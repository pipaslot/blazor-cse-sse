using System.Threading;
using System.Threading.Tasks;

namespace App.Shared.Mediator
{
    /// <summary>
    /// Replace MediatR mediator to provide exception handling
    /// </summary>
    public interface IMediator
    {
        Task<MediatorResponse<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);

        Task<MediatorResponse> Publish<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand;
    }
}

using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator
{
    /// <summary>
    ///     Command / Query dispatched
    /// </summary>
    public interface IMediator
    {
        Task<MediatorResponse<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);

        Task<MediatorResponse> Send<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand;
    }
}

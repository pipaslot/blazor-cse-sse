using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Abstractions
{
    public static class IMediatorExtensions
    {
        public static async Task<MediatorResponse> Dispatch<TCommand>(this IMediator mediator, TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            var response = await mediator.Send(command, cancellationToken);
            return response;
        }
    }
}
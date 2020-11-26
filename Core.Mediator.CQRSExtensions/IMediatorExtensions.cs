using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;

namespace Core.Mediator.CQRSExtensions
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
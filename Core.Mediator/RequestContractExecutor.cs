using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;

namespace Core.Mediator
{
    /// <summary>
    /// Server side executed receiving CommandQueryContract object through network connection
    /// </summary>
    public class RequestContractExecutor
    {
        private readonly IMediator _mediator;

        public RequestContractExecutor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<MediatorResponse> ExecuteQuery(RequestContract request, CancellationToken cancellationToken)
        {
            var queryType = Type.GetType(request.ObjectName);
            if (queryType == null)
            {
                throw new Exception($"Can not recognize type {request.ObjectName}");
            }
            var query = JsonSerializer.Deserialize(request.Json, queryType);
            if (query == null)
            {
                throw new Exception($"Can not deserialize contract as type {request.ObjectName}");
            }
            if(query is IEvent @event)
            {
                return await ExecuteEvent(@event, cancellationToken).ConfigureAwait(false);
            }
            return await ExecuteRequest(query, cancellationToken).ConfigureAwait(false);
        }

        private async Task<MediatorResponse> ExecuteEvent(IEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                return await _mediator.Fire(@event, cancellationToken);
            }
            catch (Exception e)
            {
                return new MediatorResponse(e.Message);
            }
        }

        private async Task<MediatorResponse> ExecuteRequest(object? query, CancellationToken cancellationToken)
        {
            var queryInterfaceType = typeof(IRequest<>);
            var resultType = query.GetType()
                .GetInterfaces()
                .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == queryInterfaceType)
                ?.GetGenericArguments()
                .FirstOrDefault();
            if (resultType == null)
            {
                throw new Exception($"Object {query.GetType()} is not assignable to type {queryInterfaceType }");
            }

            var method = _mediator.GetType()
                    .GetMethod(nameof(IMediator.Send))!
                .MakeGenericMethod(resultType);
            try
            {
                var task = (Task)method.Invoke(_mediator, new[] { query, cancellationToken })!;
                await task.ConfigureAwait(false);

                var resultProperty = task.GetType().GetProperty("Result");
                var result = resultProperty?.GetValue(task);
                return (MediatorResponse)result!;
            }
            catch (Exception e)
            {
                return new MediatorResponse(e.Message);
            }
        }
    }
}
